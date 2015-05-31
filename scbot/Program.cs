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
            var htmlDomainBlacklist = new[]
            {"jira", "jira.red-gate.com", "rg-jira01", "rg-jira01.red-gate.com", "redgatesupport.zendesk.com", "slack.com"};
            var persistence = new JsonFileKeyValueStore(new FileInfo("scbot.db.json"));

            var slackApi = new SlackApi(Configuration.SlackApiKey);
            var slackRtmConnection = ReconnectingSlackRealTimeMessaging.CreateAsync(
                async () => await slackApi.StartRtm());

            var teamcityApi = new TeamcityBuildApi(new JsonProxyTeamcityApi(Configuration.TeamcityApiBase));

            var time = new Time();
            var jiraApi = new CachedJiraApi(time, new JiraApi());
            var zendeskApiConnection = ReconnectingZendeskApi.CreateAsync(
                    async () => await ZendeskApi.CreateAsync(Configuration.RedgateId));
            var zendeskApi = new ErrorCatchingZendeskTicketApi(
                new ZendeskTicketApi(new CachedZendeskApi(time, await zendeskApiConnection)));

            var slackRtm = await slackRtmConnection;

            var commandParser = new SlackCommandParser("scbot", slackRtm.BotId);

            var webClient = new WebClient();

            var tcWebHooksProcessor = new TeamcityWebhooksMessageProcessor(persistence, commandParser);
            var tcWebHooksStatus = StatusWebhooksHandler.Create(time, webClient, Configuration.GithubToken);
            var webApp = TeamcityWebhooksEndpoint.Start(Configuration.TeamcityWebhooksEndpoint, new IAcceptTeamcityEvents[] {tcWebHooksProcessor, tcWebHooksStatus});

            var githubReviewer = ReviewFactory.GetProcessor(commandParser, webClient,
                Configuration.GithubToken, Configuration.GithubDefaultUser, Configuration.GithubDefaultRepo);

            var notes = new BasicFeature("notes", "save notes for later", "use `note <text>` to save a note, `notes` to list notes and `delete note <num>` to delete a specific note",
                new NoteProcessor(commandParser, new NoteApi(persistence)));
            var zdTracker = new BasicFeature("zdtracker", "track comments added to zendesk tickets", "use `track ZD#12345` to start tracking a zendesk ticket in the current channel",
                            new ZendeskTicketTracker(commandParser, persistence, zendeskApi));
            var recordreplay = new BasicFeature("recordreplay", "delete record/replay traces for a branch", "use `delete traces for <branch>` to force everything to be regenerated", new RecordReplayTraceManagement(commandParser));
            var seatingPlans = new BasicFeature("seatingplans", "find people/rooms in the building", "use `where is <search>` to search for a person or room", new SeatingPlans(commandParser, webClient));
            var webcams = new BasicFeature("webcams", "get links to webcams in the building", "use `cafcam` or `fooscam` to get the relevant webcam", new Webcams(commandParser, Configuration.WebcamAuth));
            var silly = new BasicFeature("silly", "get a random quote, class name, gif, etc", "use `quote`, `class name`, or `giphy <search>` to find something interesting", new Silly(commandParser, webClient));
            var installers = new BasicFeature("installers", "get a compare/data compare installer", "use `installer for <compare|data compare> <version>` to get a link to download the teamcity artifact for that build", new Installers(commandParser, webClient));
            var polls = new BasicFeature("polls", "run a poll to see who is wrong on the internet", "use `start poll` to start a poll", new Polls(commandParser));
            var rollbuildnumbers = new BasicFeature("rollbuildnumbers", "increment the Compare teamcity build numbers after a release", "use `roll build numbers` to increment the current Compare minor version (eg `11.1.20` -> `11.2.1`", new RollBuildNumbers(commandParser, Configuration.TeamcityCredentials));
            var features = new FeatureMessageProcessor(commandParser,
                notes,
                zdTracker,
                recordreplay,
                seatingPlans,
                webcams,
                silly,
                installers,
                polls,
                rollbuildnumbers
                );

            var processor =
                new ErrorCatchingMessageProcessor(
                    new ConcattingMessageProcessor(
                        new CompositeMessageProcessor(
                            features,
                            new JiraBugProcessor(commandParser, jiraApi),
                            new JiraLabelSuggester(commandParser, jiraApi),
                            new ZendeskTicketProcessor(zendeskApi),
                            //new HtmlTitleProcessor(new HtmlTitleParser(webClient), htmlDomainBlacklist),
                            //new TeamcityBuildTracker(commandParser, persistence, teamcityApi),
                            tcWebHooksProcessor,
                            githubReviewer)));

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
