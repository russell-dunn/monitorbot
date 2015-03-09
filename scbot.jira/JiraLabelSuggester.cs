using scbot.core.bot;
using scbot.jira.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.core.utils;

namespace scbot.jira
{
    public class JiraLabelSuggester : IMessageProcessor
    {
        private readonly ICommandParser m_CommandParser;
        private readonly IJiraApi m_JiraApi;

        public JiraLabelSuggester(ICommandParser commandParser, IJiraApi jiraApi)
        {
            m_CommandParser = commandParser;
            m_JiraApi = jiraApi;
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toSuggest;
            if (m_CommandParser.TryGetCommand(message, "suggest labels for", out toSuggest))
            {
                var bug = m_JiraApi.FromId(toSuggest).Result;
                if (bug == null)
                {
                    return Response.ToMessage(message, "Couldn't find bug " + toSuggest);
                }
                return Response.ToMessage(message, String.Join(" ", SuggestionsFor(bug).Distinct()));
            }
            return MessageResult.Empty;
        }


        private IEnumerable<string> SuggestionsFor(JiraBug bug)
        {
            var titleSuggestions = new Dictionary<string, string>
            {
                { "OutOfMemoryException", "bugtype:oom" },
                { "ErrorsOccurredDuringScriptFileParsingException", "bugtype:parsefail" },
                { "NullReferenceException", "bugtype:nullref" },
                { "NotImplementedException", "bugtype:notimplementedexception" },
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
                { "CompareTable(", "bugtype:comparisonfail" },
                { "BackupReader", "feature:backupreader" },
            };

            foreach (var suggestion in descriptionSuggestions)
            {
                if (bug.Description.Contains(suggestion.Key))
                {
                    yield return suggestion.Value;
                }
            }
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }
    }
}
