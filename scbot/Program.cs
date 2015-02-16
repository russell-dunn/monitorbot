using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using scbot.processors;
using scbot.services;
using scbot.slack;
using slowtests;

namespace scbot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var htmlDomainBlacklist = new[]
            {"jira", "jira.red-gate.com", "rg-jira01", "rg-jira01.red-gate.com", "redgatesupport.zendesk.com"};
            var commandParser = new SlackCommandParser("scbot", "U03JWF43N" /*TODO*/);
            var persistence = new JsonFileKeyValueStore(new FileInfo("scbot.db.json"));

            var slackApi = new SlackApi(Configuration.SlackApiKey);
            var slackRtmConnection = slackApi.StartRtm();

            var time = new Time();
            var jiraApi = new CachedJiraApi(time, new JiraApi());
            var zendeskApiConnection = ZendeskApi.CreateAsync(Configuration.RedgateId);
            var zendeskApi = new CachedZendeskApi(time, await zendeskApiConnection);

            var processor =
                new ErrorCatchingMessageProcessor(
                    new ConcattingMessageProcessor(
                        new CompositeMessageProcessor(
                            new NoteProcessor(commandParser, new NoteApi(persistence)),
                            new JiraBugProcessor(jiraApi),
                            new ZendeskTicketProcessor(zendeskApi),
                            new HtmlTitleProcessor(new HtmlTitleParser(), htmlDomainBlacklist))));

            var bot = new Bot(processor);

            var slackRtm = await slackRtmConnection;
            var handler = new SlackMessageHandler(bot, slackRtm.BotId);
            var cancellationToken = new CancellationToken();
            while (true)
            {
                var nextMessage = slackRtm.Receive(cancellationToken).Result;
                var result = handler.Handle(nextMessage);
                foreach (var response in result.Responses)
                {
                    slackApi.PostMessage(response).Wait(cancellationToken);
                }
            }
        }
    }
}
