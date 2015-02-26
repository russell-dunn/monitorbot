using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace scbot.zendesk.services
{
    public class ZendeskApi : IZendeskApi
    {
        private readonly CookieContainer m_CookieJar;

        private ZendeskApi(CookieContainer cookieJar)
        {
            m_CookieJar = cookieJar;
        }

        public static ZendeskApi Create(string redgateId)
        {
            return CreateAsync(redgateId).Result;
        }

        public static async Task<ZendeskApi> CreateAsync(string redgateId)
        {
            var userAndPass = redgateId.Split(new[] { ':' }, 2);
            var username = HttpUtility.UrlEncode(userAndPass[0]);
            var password = HttpUtility.UrlEncode(userAndPass[1]);

            // We need to collect some tasty cookies to bribe the zendesk API into letting us make requests
            var cookieJar = new CookieContainer();

            using (var client = new CookieAwareWebClient(cookieJar))
            {
                Console.WriteLine("getting login cookie");
                var r1 = await client.DownloadStringTaskAsync("https://redgatesupport.zendesk.com/login?return_to=https%3A//redgatesupport.zendesk.com/");
                Console.WriteLine("logging into rgid");
                var r2 = await client.DownloadStringTaskAsync(string.Format("https://authentication.red-gate.com/openid/login?callback=c&emailAddress={0}&password={1}&_=5", username, password));
                if (!r2.StartsWith("c(") && r2.EndsWith(")")) throw new Exception("expected jsonp callback called c()");
                var fixedJson = r2.Substring("c(".Length, r2.Length - "c(".Length - ")".Length);
                var redirectTo = (string)Json.Decode(fixedJson).redirectTo;
                Console.WriteLine("redirect back to zdesk");
                var r3 = await client.DownloadStringTaskAsync(redirectTo);
                Console.WriteLine("downloading a real issue to get api access (for some reason this seems to make future requests more reliable)");
                var real = await client.DownloadStringTaskAsync("https://redgatesupport.zendesk.com/agent/tickets/36414");
            }

            return new ZendeskApi(cookieJar);
        }

        private async Task<dynamic> Get(string endpoint) 
        {
            const string apiBase = "https://redgatesupport.zendesk.com/api/v2/";
            using (var client = new CookieAwareWebClient(m_CookieJar))
            {
                return Json.Decode(await client.DownloadStringTaskAsync(apiBase+endpoint));
            }
        }

        public async Task<dynamic> Ticket(string ticketId)
        {
            return await Get(string.Format("tickets/{0}.json", ticketId));
        }

        public async Task<dynamic> Comments(string ticketId)
        {
            return await Get(string.Format("tickets/{0}/comments.json", ticketId));
        }

        public async Task<dynamic> User(string userId) 
        { 
            return await Get(string.Format("users/{0}.json", userId));
        }
    }
}
