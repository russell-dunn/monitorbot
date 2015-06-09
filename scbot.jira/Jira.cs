using scbot.core.bot;
using scbot.core.meta;
using scbot.core.utils;
using scbot.jira.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.jira
{
    public static class Jira
    {
        public static IFeature Create(ICommandParser commandParser)
        {
            var time = new Time();
            var jiraApi = new CachedJiraApi(time, new JiraApi());
            var processor = new CompositeMessageProcessor(
                            new JiraBugProcessor(commandParser, jiraApi),
                            new JiraLabelSuggester(commandParser, jiraApi));
            return new BasicFeature("jira", "track jira tickets mentioned in chat", "", processor);
        }
    }
}
