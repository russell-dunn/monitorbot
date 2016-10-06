using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using monitorbot.htmltitles;
using monitorbot.htmltitles.services;
using monitorbot.jira.services;
using monitorbot.notes.services;
using monitorbot.teamcity.services;
using monitorbot.teamcity.webhooks;
using monitorbot.zendesk;
using monitorbot.zendesk.services;
using monitorbot.teamcity.webhooks.endpoint;
using monitorbot.teamcity.webhooks.githubstatus;
using System.Diagnostics;
using System.Collections.Generic;
using monitorbot.core.bot;
using monitorbot.core.meta;
using monitorbot.core.persistence;
using monitorbot.core.utils;
using monitorbot.games;
using monitorbot.jira;
using monitorbot.labelprinting;
using monitorbot.logging;
using monitorbot.notes;
using monitorbot.polls;
using monitorbot.release;
using monitorbot.review;
using monitorbot.rg;
using monitorbot.silly;
using monitorbot.slack;

namespace monitorbot
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
            var aliasList = GetAliasList(slackRtm.InstanceInfo.Users);

            var commandParser = new SlackCommandParser("scbot", slackRtm.InstanceInfo.BotId);

            var webClient = new WebClient();

            var features = new FeatureMessageProcessor(commandParser,
                NoteProcessor.Create(commandParser, persistence),
                //ZendeskTicketTracker.Create(commandParser, persistence, configuration),
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
                CompareTeamEmails.Create(commandParser, configuration),
                GamesProcessor.Create(commandParser, gamesPersistence, aliasList)
                );

            var pasteBin = new HasteServerPasteBin(webClient, configuration.Get("haste-server-url"));

            var newChannelNotificationsChannel = configuration.GetWithDefault("new-channels-notification-channel", null);
            var newChannelProcessor = GetNewChannelProcessor(newChannelNotificationsChannel);

            var bot = new Bot(
                new ErrorReportingMessageProcessor(
                    new ConcattingMessageProcessor(features),
                    pasteBin),
                newChannelProcessor);

            var handler = new SlackMessageHandler(bot, slackRtm.InstanceInfo.BotId);

            MainLoop(slackRtm, handler, slackApi, cancel);
        }

        private static IAliasList GetAliasList(IEnumerable<SlackUser> users)
        {
            var aliasList = new AliasList();
            foreach (var user in users)
            {
                aliasList.AddAlias(user.SlackId, user.DisplayName, new[] {
                    user.UserName,
                    string.Format("<@{0}>", user.SlackId)
                });
            }
            return aliasList;
        }

        private static INewChannelProcessor GetNewChannelProcessor(string newChannelNotificationsChannel)
        {
            var newChannelProcessor = !String.IsNullOrWhiteSpace(newChannelNotificationsChannel)
                ? (INewChannelProcessor) new TellSlackChannelAboutNewChannels(newChannelNotificationsChannel)
                : new NullNewChannelProcessor();

            Trace.TraceInformation(string.Format("new channel processor: {0} [{1}]", newChannelProcessor.GetType().Name,
                newChannelNotificationsChannel));

            return newChannelProcessor;
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
