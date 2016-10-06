using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using monitorbot.core.bot;
using monitorbot.core.compareengine;
using monitorbot.core.persistence;
using monitorbot.teamcity.webhooks.endpoint;
using monitorbot.teamcity.webhooks.tests;
using monitorbot.core.utils;

namespace monitorbot.teamcity.webhooks
{
    public class TeamcityWebhooksMessageProcessor : IMessageProcessor, IAcceptTeamcityEvents
    {
        private readonly IHashPersistenceApi<Tracked<Build>> m_BuildPersistence;
        private readonly IHashPersistenceApi<Tracked<Branch>> m_BranchPersistence;
        private readonly ICommandParser m_CommandParser;
        private readonly ConcurrentQueue<TeamcityEvent> m_Queue = new ConcurrentQueue<TeamcityEvent>();

        private static readonly Regex s_TrackRegex = new Regex(@"(?<eventType>(breakages))?\s*(for\s*)?(?<trackType>(build|branch))\s+(?<trackItem>([0-9]{5,10}|[a-z/\-_0-9]+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly TeamcityEventHandler m_TeamcityEventHandler;

        internal TeamcityWebhooksMessageProcessor(IHashPersistenceApi<Tracked<Build>> buildPersistence, IHashPersistenceApi<Tracked<Branch>> branchPersistence, ICommandParser commandParser)
        {
            m_BuildPersistence = buildPersistence;
            m_BranchPersistence = branchPersistence;
            m_CommandParser = commandParser;
            m_TeamcityEventHandler = new TeamcityEventHandler();
        }

        public TeamcityWebhooksMessageProcessor(IKeyValueStore kvs, ICommandParser commandParser)
         : this(new HashPersistenceApi<Tracked<Build>>(kvs, "tcwh-tracked-builds"),
                new HashPersistenceApi<Tracked<Branch>>(kvs, "tcwh-tracked-branches"),
                commandParser)
        {
        }

        public MessageResult ProcessTimerTick()
        {
            var result = new List<Response>();
            TeamcityEvent nextEvent;
            var trackedBuilds = m_BuildPersistence.GetValues();
            var trackedBranches = m_BranchPersistence.GetValues();
            while (m_Queue.TryDequeue(out nextEvent))
            {
                result.AddRange(m_TeamcityEventHandler.GetResponseTo(nextEvent, trackedBuilds, trackedBranches));
            }
            return new MessageResult(result);
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack;
            Match trackMatch;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_TrackRegex.TryMatch(toTrack, out trackMatch))
            {
                return ProcessMessage(message, trackMatch);
            }
            return MessageResult.Empty;
        }

        private MessageResult ProcessMessage(Message message, Match regexMatch)
        {
            return ProcessMessage(message, regexMatch.Group("trackType"), regexMatch.Group("trackItem"), regexMatch.Group("eventType"));
        }

        internal MessageResult ProcessMessage(Message message, string trackType, string trackItem, string eventType)
        {
            switch (trackType)
            {
                case "build": return TrackBuild(message, trackItem);
                case "branch": return TrackBranch(message, trackItem, eventType);
                default: return MessageResult.Empty;
            }
        }

        private MessageResult TrackBranch(Message message, string branch, string eventType)
        {
            var parsedEventType = GetEventTypes(eventType);
            m_BranchPersistence.Set(KeyFor(message.Channel, branch), new Tracked<Branch>(new Branch(parsedEventType, branch), message.Channel));
            return Response.ToMessage(message, string.Format("Now tracking {0} for branch {1}", eventType, branch));
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
            m_BuildPersistence.Set(KeyFor(message.Channel, buildId), new Tracked<Build>(new Build(buildId), message.Channel));
            return Response.ToMessage(message, string.Format("Now tracking build#{0}", buildId));
        }

        public void Accept(TeamcityEvent teamcityEvent)
        {
            m_Queue.Enqueue(teamcityEvent);
        }

        private static string KeyFor(string channel, string thing)
        {
            return string.Format("{0}:::{1}", channel, thing);
        }
    }
}
