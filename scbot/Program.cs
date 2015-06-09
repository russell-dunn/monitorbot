using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using scbot.core.bot;
using scbot.core.meta;
using scbot.core.persistence;
using scbot.core.utils;
using scbot.htmltitles;
using scbot.htmltitles.services;
using scbot.jira;
using scbot.jira.services;
using scbot.notes;
using scbot.notes.services;
using scbot.slack;
using scbot.teamcity.services;
using scbot.teamcity.webhooks;
using scbot.zendesk;
using scbot.zendesk.services;
using scbot.review;
using scbot.teamcity.webhooks.endpoint;
using scbot.teamcity.webhooks.githubstatus;
using scbot.logging;
using scbot.polls;
using scbot.release;
using scbot.rg;
using scbot.silly;
using scbot.labelprinting;

namespace scbot
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            using (var dash = new LoggingDashboard(Configuration.LoggingEndpoint))
            {
                MainAsync().Wait();
            }
        }

        private static async Task MainAsync()
        {
            var persistence = new JsonFileKeyValueStore(new FileInfo("scbot.db.json"));

            var slackApi = new SlackApi(Configuration.SlackApiKey);

            var slackRtm = await (ReconnectingSlackRealTimeMessaging.CreateAsync(
                async () => await slackApi.StartRtm()));

            var commandParser = new SlackCommandParser("scbot", slackRtm.BotId);

            var webClient = new WebClient();

            var features = new FeatureMessageProcessor(commandParser,
                NoteProcessor.Create(commandParser, persistence),
                ZendeskTicketTracker.Create(commandParser, persistence),
                RecordReplayTraceManagement.Create(commandParser),
                SeatingPlans.Create(commandParser, webClient),
                Webcams.Create(commandParser, Configuration.WebcamAuth),
                Silly.Create(commandParser, webClient),
                Installers.Create(commandParser, webClient),
                Polls.Create(commandParser),
                RollBuildNumbers.Create(commandParser, Configuration.TeamcityCredentials),
                ReviewFactory.Create(commandParser, webClient, Configuration.GithubToken, Configuration.GithubDefaultUser, Configuration.GithubDefaultRepo),
                LabelPrinting.Create(commandParser, webClient, Configuration.GithubDefaultUser, Configuration.GithubToken, Configuration.LabelPrinterApiUrl)
                );

            var processor =
                new ErrorCatchingMessageProcessor(
                    new ConcattingMessageProcessor(
                        new CompositeMessageProcessor(
                            features,
                            Jira.Create(commandParser)
                            )));

            var bot = new Bot(processor);

            var handler = new SlackMessageHandler(bot, slackRtm.BotId);
            var cancellationToken = new CancellationToken();

            MainLoop(slackRtm, handler, slackApi, cancellationToken);
        }

        private static void MainLoop(ISlackRealTimeMessaging slackRtm, SlackMessageHandler handler, SlackApi slackApi, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var nextMessage = slackRtm.Receive(cancellationToken);
                while (!nextMessage.Wait(TimeSpan.FromSeconds(10)))
                {
                    var tickResult = handler.HandleTimerTick();
                    Console.Write(".");
                    DoResponse(slackApi, tickResult, cancellationToken);
                }

                var messageResult = handler.Handle(nextMessage.Result);
                DoResponse(slackApi, messageResult, cancellationToken);
            }
        }

        private static void DoResponse(SlackApi slackApi, MessageResult result, CancellationToken cancellationToken)
        {
            foreach (var response in result.Responses)
            {
                slackApi.PostMessage(response).Wait(cancellationToken);
            }
        }
    }
}
