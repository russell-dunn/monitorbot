using System.Linq;
using System.Text.RegularExpressions;
using scbot.services;

namespace scbot
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
            var id = s_BugRegex.Matches(message.MessageText).Cast<Match>().FirstOrDefault();
            if (id == null) return MessageResult.Empty;
            var bug = m_JiraApi.FromId(id.Groups[0].ToString()).Result;
            var messageText = string.Format("<{0}|{1}> | {2} | {3} {4}", "https://jira.red-gate.com/browse/" + id, id, bug.Title, bug.CommentCount, bug.CommentCount == 1 ? "comment" : "comments");
            return new MessageResult(new[] { Response.ToMessage(message, messageText), });
        }
    }
}