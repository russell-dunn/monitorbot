using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using scbot.core.bot;
using scbot.core.compareengine;
using scbot.core.persistence;
using scbot.core.utils;
using scbot.teamcity.webhooks.tests;

namespace scbot.teamcity.webhooks
{
    public class TeamcityWebhooksMessageProcessor : IMessageProcessor, IAcceptTeamcityEvents
    {
        private readonly IListPersistenceApi<Tracked<Build>> m_BuildPersistence;
        private readonly IListPersistenceApi<Tracked<Branch>> m_BranchPersistence;
        private readonly ICommandParser m_CommandParser;
        private  readonly ConcurrentQueue<TeamcityEvent> m_Queue = new ConcurrentQueue<TeamcityEvent>();

        private static readonly Regex s_TrackRegex = new Regex(@"(?<eventType>(breakages))?\s*(for\s*)?(?<trackType>(build|branch))\s+(?<trackItem>([0-9]{5,10}|[a-z/\-_0-9]+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly TeamcityEventHandler m_TeamcityEventHandler;

        internal TeamcityWebhooksMessageProcessor(IListPersistenceApi<Tracked<Build>> buildPersistence, IListPersistenceApi<Tracked<Branch>> branchPersistence, ICommandParser commandParser)
        {
            m_BuildPersistence = buildPersistence;
            m_BranchPersistence = branchPersistence;
            m_CommandParser = commandParser;
            m_TeamcityEventHandler = new TeamcityEventHandler();
        }

        public TeamcityWebhooksMessageProcessor(IKeyValueStore kvs, ICommandParser commandParser)
         : this(new ListPersistenceApi<Tracked<Build>>(kvs, "tcwh-tracked-builds"), 
                new ListPersistenceApi<Tracked<Branch>>(kvs, "tcwh-tracked-branches"), 
                commandParser)
        {
        }

        public MessageResult ProcessTimerTick()
        {
            var result = new List<Response>();
            TeamcityEvent nextEvent;
            var trackedBuilds = m_BuildPersistence.ReadList();
            var trackedBranches = m_BranchPersistence.ReadList();
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
            m_BranchPersistence.AddToList(new Tracked<Branch>(new Branch(parsedEventType, branch), message.Channel));
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
            m_BuildPersistence.AddToList(new Tracked<Build>(new Build(buildId), message.Channel));
            return new MessageResult(new[]{Response.ToMessage(message, string.Format("Now tracking build#{0}", buildId))});
        }

        public void Accept(TeamcityEvent teamcityEvent)
        {
            m_Queue.Enqueue(teamcityEvent);
        }
    }
}
