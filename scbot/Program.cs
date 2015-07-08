using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using scbot.core.bot;
using scbot.core.meta;
using scbot.core.persistence;
using scbot.core.utils;
using scbot.games;
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
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = Configuration.Load();
            using (var dash = new LoggingDashboard(configuration.Get("logging-dashboard-endpoint")))
            {
                MainAsync(configuration, new CancellationToken()).Wait();
            }
        }

        public static async Task MainAsync(Configuration configuration, CancellationToken cancel)
        {
            var persistence = new JsonFileKeyValueStore(new FileInfo(configuration.Get("db-file-location")));
            var gamesPersistence = new JsonFileKeyValueStore(new FileInfo(configuration.Get("games-db-location")));

            var slackApi = new SlackApi(configuration.Get("slack-api-key"));

            var slackRtm = await (ReconnectingSlackRealTimeMessaging.CreateAsync(
                async () => await slackApi.StartRtm()));

            var commandParser = new SlackCommandParser("scbot", slackRtm.BotId);

            var webClient = new WebClient();

            var features = new FeatureMessageProcessor(commandParser,
                NoteProcessor.Create(commandParser, persistence),
                ZendeskTicketTracker.Create(commandParser, persistence, configuration),
                RecordReplayTraceManagement.Create(commandParser),
                SeatingPlans.Create(commandParser, webClient),
                Webcams.Create(commandParser, configuration),
                Silly.Create(commandParser, webClient),
                Installers.Create(commandParser, webClient),
                Polls.Create(commandParser),
                RollBuildNumbers.Create(commandParser, configuration),
                ReviewFactory.Create(commandParser, webClient, configuration),
                LabelPrinting.Create(commandParser, webClient, configuration),
                Jira.Create(commandParser),
                GamesProcessor.Create(commandParser, gamesPersistence)
                );

            var bot = new Bot(
                new ErrorCatchingMessageProcessor(
                    new ConcattingMessageProcessor(
                            features)));

            var handler = new SlackMessageHandler(bot, slackRtm.BotId);

            MainLoop(slackRtm, handler, slackApi, cancel);
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
