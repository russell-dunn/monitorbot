using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace scbot.services
{
    public class ZendeskTicketApi : IZendeskTicketApi
    {
        private readonly CookieContainer m_CookieJar;

        private ZendeskTicketApi(CookieContainer cookieJar)
        {
            m_CookieJar = cookieJar;
        }

        public static ZendeskTicketApi Create(string redgateId)
        {
            return CreateAsync(redgateId).Result;
        }

        public static async Task<ZendeskTicketApi> CreateAsync(string redgateId)
        {
            var userAndPass = redgateId.Split(new[] {':'}, 2);
            var username = HttpUtility.UrlEncode(userAndPass[0]);
            var password = HttpUtility.UrlEncode(userAndPass[1]);

            // We need to collect some tasty cookies to bribe the zendesk API into letting us make requests
            var cookieJar = new CookieContainer();

            using (var client = new CookieAwareWebClient(cookieJar))
            {
                Console.WriteLine("getting login cookie");
                var r1 = await client.DownloadStringTaskAsync("https://redgatesupport.zendesk.com/login?return_to=https%3A//redgatesupport.zendesk.com/");
                Console.WriteLine("logging into rgid");
                var r2 = await client.DownloadStringTaskAsync(string.Format( "https://authentication.red-gate.com/openid/login?callback=c&emailAddress={0}&password={1}&_=5", username, password));
                if (!r2.StartsWith("c(") && r2.EndsWith(")")) throw new Exception("expected jsonp callback called c()");
                var fixedJson = r2.Substring("c(".Length, r2.Length - "c(".Length - ")".Length);
                var redirectTo = (string) Json.Decode(fixedJson).redirectTo;
                Console.WriteLine("redirect back to zdesk");
                var r3 = await client.DownloadStringTaskAsync(redirectTo);
                Console.WriteLine( "downloading a real issue to get api access (for some reason this seems to make future requests more reliable)");
                var real = await client.DownloadStringTaskAsync("https://redgatesupport.zendesk.com/agent/tickets/36414");
            }

            return new ZendeskTicketApi(cookieJar);
        }

        public async Task<ZendeskTicket> FromId(string id)
        {
            const string apiBase = "https://redgatesupport.zendesk.com/api/v2/";
            using (var client = new CookieAwareWebClient(m_CookieJar))
            {
                var ticket = Json.Decode(await client.DownloadStringTaskAsync(string.Format("{0}tickets/{1}.json", apiBase, id)));
                var comments = Json.Decode(await client.DownloadStringTaskAsync(string.Format("{0}tickets/{1}/comments.json", apiBase, id)));
                return new ZendeskTicket(id, ticket.ticket.subject, ticket.ticket.status, comments.count);
            }
        }
    }

}