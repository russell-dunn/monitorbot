using System.Linq;
using System.Text.RegularExpressions;
using scbot.core.bot;
using scbot.jira.services;
using scbot.core.utils;
using System;
using System.Collections.Generic;

namespace scbot.jira
{
    public class JiraBugProcessor : IMessageProcessor
    {
        private readonly IJiraApi m_JiraApi;
        private static readonly Regex s_BugRegex = new Regex(@"[a-z]{2,5}-[0-9]{1,7}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly string s_JiraLogo = "https://slack.global.ssl.fastly.net/14542/img/services/jira_48.png";
        private readonly ICommandParser m_CommandParser;

        public JiraBugProcessor(ICommandParser commandParser, IJiraApi jiraApi)
        {
            m_CommandParser = commandParser;
            m_JiraApi = jiraApi;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toSuggest;
            if (m_CommandParser.TryGetCommand(message, "suggest labels for", out toSuggest))
            {
                var bug = m_JiraApi.FromId(toSuggest).Result;
                if (bug == null)
                {
                    return new MessageResult(new[] { Response.ToMessage(message, "Couldn't find bug " + toSuggest) });
                }
                return new MessageResult(new[] { Response.ToMessage(message, String.Join(" ", SuggestionsFor(bug).Distinct())) });
            }

            return AddLinksForMentionedBugs(message);
        }

        private IEnumerable<string> SuggestionsFor(JiraBug bug)
        {
            var titleSuggestions = new Dictionary<string, string>
            {
                { "OutOfMemoryException", "bugtype:oom" },
                { "ErrorsOccurredDuringScriptFileParsingException", "bugtype:parsefail" },
                { "NullReferenceException", "bugtype:nullref" },
            };

            foreach (var suggestion in titleSuggestions)
            {
                if (bug.Title.Contains(suggestion.Key))
                {
                    yield return suggestion.Value;
                }
            }

            var descriptionSuggestions = new Dictionary<string, string>
            {
                { "\"MethodTypeName\": \"[RedGate.SQLCompare.Engine]", "repo:sqlcompareengine" },
                { "\"MethodTypeName\": \"[RedGate.SQLDataCompare.Engine]", "repo:sqldatacompareengine" },
                { "\"MethodTypeName\": \"[RedGate.Shared.SQL]", "repo:sharedsql" },
                { "\"MethodTypeName\": \"[RedGate.SQLCompare.ASTParser]", "repo:sqlcompareparser" },
                { "\"MethodTypeName\": \"[RedGate.SQLToolsUI]", "repo:sqltoolsui" },
                { "\"MethodTypeName\": \"[RedGate.BackupReader]", "repo:sqlbackupreader" },
                { "RedGate.SQLSourceControl.Engine", "seenin:soc" },
                { "ToCommitChangeSet", "feature:soc-commit" },
                { "ToRetrieveChangeSet", "feature:soc-getlatest" },
            };

            foreach (var suggestion in descriptionSuggestions)
            {
                if (bug.Description.Contains(suggestion.Key))
                {
                    yield return suggestion.Value;
                }
            }
        }

        private MessageResult AddLinksForMentionedBugs(Message message)
        {
            var matches = s_BugRegex.Matches(message.MessageText).Cast<Match>();
            var ids = matches.Select(x => x.Groups[0].ToString()).Distinct();
            var bugs = ids.Select(id => m_JiraApi.FromId(id).Result).Where(x => x != null);
            var messageTexts = bugs.Select(FormatBug);
            return new MessageResult(messageTexts.Select(x => Response.ToMessage(message, x, s_JiraLogo)).ToList());
        }

        private static string FormatBug(JiraBug bug)
        {
            return string.Format("<{0}|{1}> | {2} | {3} | {4} {5}", "https://jira.red-gate.com/browse/" + bug.Id, bug.Id,
                bug.Title, bug.Status, bug.CommentCount, bug.CommentCount == 1 ? "comment" : "comments");
        }
    }
}