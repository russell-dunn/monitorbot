using System.Linq;
using System.Text.RegularExpressions;
using scbot.services;

namespace scbot.processors
{
    public class JiraBugProcessor : IMessageProcessor
    {
        private readonly IJiraApi m_JiraApi;
        private static readonly Regex s_BugRegex = new Regex(@"[a-z]{2,5}-[0-9]{1,7}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public JiraBugProcessor(IJiraApi jiraApi)
        {
            m_JiraApi = jiraApi;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            var matches = s_BugRegex.Matches(message.MessageText).Cast<Match>();
            var ids = matches.Select(x => x.Groups[0].ToString()).Distinct();
            var bugs = ids.Select(id => m_JiraApi.FromId(id).Result).Where(x => x != null);
            var messageTexts = bugs.Select(FormatBug);
            return new MessageResult(messageTexts.Select(x => Response.ToMessage(message, x)).ToList());
        }

        private static string FormatBug(JiraBug bug)
        {
            return string.Format("<{0}|{1}> | {2} | {3} | {4} {5}", "https://jira.red-gate.com/browse/" + bug.Id, bug.Id,
                bug.Title, bug.Status, bug.CommentCount, bug.CommentCount == 1 ? "comment" : "comments");
        }
    }
}