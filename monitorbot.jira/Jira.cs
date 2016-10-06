using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.bot;
using monitorbot.core.meta;
using monitorbot.core.utils;
using monitorbot.jira.services;

namespace monitorbot.jira
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
