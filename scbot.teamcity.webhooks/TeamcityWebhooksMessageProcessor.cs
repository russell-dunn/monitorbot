using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.Owin.Hosting;
using Owin;
using scbot.bot;
using scbot.services.compareengine;
using scbot.services.persistence;
using scbot.teamcity.webhooks;
using scbot.teamcity.webhooks.tests;
using scbot.utils;

namespace scbot.services.teamcity
{
    public class TeamcityWebhooksMessageProcessor : IMessageProcessor, IDisposable
    {
        private const string c_TrackedBuilds = "tcwh-tracked-builds";
        private const string c_TrackedBranches = "tcwh-tracked-branches";
        private readonly IListPersistenceApi<Tracked<Build>> m_BuildPersistence;
        private readonly IListPersistenceApi<Tracked<Branch>> m_BranchPersistence;
        private readonly ICommandParser m_CommandParser;
        private readonly IDisposable m_WebApp;
        // hack communication between OWIN instance and bot-created instance
        private static readonly ConcurrentQueue<string> s_Queue = new ConcurrentQueue<string>();

        private static readonly Regex s_TrackRegex = new Regex(@"(?<eventType>(breakages))?\s*(for\s*)?(?<trackType>(build|branch))\s+(?<trackItem>([0-9]{5,10}|[a-z/\-_0-9]+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly TeamcityEventHandler m_TeamcityEventHandler;

        private TeamcityWebhooksMessageProcessor(IListPersistenceApi<Tracked<Build>> buildPersistence, IListPersistenceApi<Tracked<Branch>> branchPersistence, ICommandParser commandParser, IDisposable webApp)
        {
            m_BuildPersistence = buildPersistence;
            m_BranchPersistence = branchPersistence;
            m_CommandParser = commandParser;
            m_WebApp = webApp;
            m_TeamcityEventHandler = new TeamcityEventHandler();
        }

        [Obsolete("Required by OWIN to call Configuration", true)]
        public TeamcityWebhooksMessageProcessor() { }

        internal static TeamcityWebhooksMessageProcessor Start(IListPersistenceApi<Tracked<Build>> buildPersistence, IListPersistenceApi<Tracked<Branch>> branchPersistence, ICommandParser commandParser, string binding)
        {
            var webApp = WebApp.Start<TeamcityWebhooksMessageProcessor>(binding);
            return new TeamcityWebhooksMessageProcessor(buildPersistence, branchPersistence, commandParser, webApp);
        }

        public static IMessageProcessor Start(IKeyValueStore kvs, ICommandParser commandParser, string binding)
        {
            return Start(new ListPersistenceApi<Tracked<Build>>(kvs), 
                new ListPersistenceApi<Tracked<Branch>>(kvs), 
                commandParser, 
                binding);
        }

        public void Configuration(IAppBuilder app)
        {
            app.Run(owinContext =>
            {
                var body = new StreamReader(owinContext.Request.Body).ReadToEnd();
                s_Queue.Enqueue(body);

                owinContext.Response.ContentType = "text/plain";
                return owinContext.Response.WriteAsync("thanks");
            });
        }

        public MessageResult ProcessTimerTick()
        {
            var result = new List<Response>();
            string nextJson;
            var trackedBuilds = m_BuildPersistence.ReadList(c_TrackedBuilds);
            var trackedBranches = m_BranchPersistence.ReadList(c_TrackedBranches);
            while (s_Queue.TryDequeue(out nextJson))
            {
                var teamcityEvent = ParseTeamcityEvent(nextJson);
                result.AddRange(m_TeamcityEventHandler.GetResponseTo(teamcityEvent, trackedBuilds, trackedBranches));
            }
            return new MessageResult(result);
        }

        private static TeamcityEvent ParseTeamcityEvent(string eventJson)
        {
            try
            {
                var build = Json.Decode(eventJson).build;
                return new TeamcityEvent(
                    build.notifyType,
                    build.buildId,
                    build.buildTypeId,
                    build.buildName,
                    build.buildResultDelta,
                    build.branchName
                    );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack;
            Match trackMatch;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_TrackRegex.TryMatch(toTrack, out trackMatch))
            {
                var trackType = trackMatch.Groups["trackType"].ToString();
                var trackItem = trackMatch.Groups["trackItem"].ToString();
                var eventType = trackMatch.Groups["eventType"].ToString();
                switch (trackType)
                {
                    case "build": return TrackBuild(message, trackItem);
                    case "branch": return TrackBranch(message, trackItem, eventType);
                }
            }
            return MessageResult.Empty;
        }

        private MessageResult TrackBranch(Message message, string branch, string eventType)
        {
            var parsedEventType = GetEventTypes(eventType);
            m_BranchPersistence.AddToList(c_TrackedBranches, new Tracked<Branch>(new Branch(parsedEventType, branch), message.Channel));
            return new MessageResult(new[]{Response.ToMessage(message, string.Format("Now tracking {0} for branch {1}", eventType, branch))});
        }

        private TeamcityEventTypes GetEventTypes(string eventType)
        {
            switch (eventType)
            {
                case "breakage":
                case "breakages": 
                    return TeamcityEventTypes.BreakingBuilds;
            }
            return TeamcityEventTypes.All;
        }

        private MessageResult TrackBuild(Message message, string buildId)
        {
            m_BuildPersistence.AddToList(c_TrackedBuilds, new Tracked<Build>(new Build(buildId), message.Channel));
            return new MessageResult(new[]{Response.ToMessage(message, string.Format("Now tracking build#{0}", buildId))});
        }

        public void Dispose()
        {
            m_WebApp.Dispose();
        }
    }
}
