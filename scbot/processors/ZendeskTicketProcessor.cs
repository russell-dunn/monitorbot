using System.Linq;
using System.Text.RegularExpressions;
using scbot.services;

namespace scbot.processors
{
    public class ZendeskTicketProcessor : IMessageProcessor
    {
        private readonly IZendeskApi m_ZendeskApi;
        private static readonly Regex s_ZendeskIssueRegex = new Regex(@"(?:ZD#(?<id>\d{5})|\<https\:\/\/redgatesupport.zendesk.com\/agent\/tickets\/(?<id>\d{5})\>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ZendeskTicketProcessor(IZendeskApi zendeskApi)
        {
            m_ZendeskApi = zendeskApi;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            var matches = s_ZendeskIssueRegex.Matches(message.MessageText).Cast<Match>();
            var ids = matches.Select(x => x.Groups["id"].ToString());
            var bugs = ids.Select(x => m_ZendeskApi.FromId(x).Result);
            var responses = bugs.Select(x => Response.ToMessage(message, FormatTicket(x)));
            return new MessageResult(responses.ToList());
        }

        private static string FormatTicket(ZendeskTicket x)
        {
            return string.Format("<{0}|ZD#{1}> | {2} | {3} | {4} {5}",
                "https://redgatesupport.zendesk.com/agent/tickets/" + x.Id, x.Id, x.Description,
                x.Status, x.CommentCount, x.CommentCount == 1 ? "comment" : "comments");
        }
    }
}