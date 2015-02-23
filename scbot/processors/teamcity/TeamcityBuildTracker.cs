using scbot;
using scbot.services;
using scbot.slack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using scbot.processors.teamcity;
using scbot.services.compareengine;

namespace fasttests.teamcity
{
    public class TeamcityBuildTracker : IMessageProcessor
    {
        private readonly SlackCommandParser m_CommandParser;
        private readonly ListPersistenceApi<TrackedTeamcityBuild> m_Persistence;
        private readonly ITeamcityBuildApi m_TeamcityBuildApi;
        private static readonly Regex s_TeamcityBuildRegex = new Regex(@"^build#(?<id>\d{5,10})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string c_PersistenceKey = "tracked-tc-builds";
        private readonly TeamcityBuildCompareEngine m_TeamcityBuildCompareEngine;

        public TeamcityBuildTracker(scbot.slack.SlackCommandParser commandParser, scbot.services.InMemoryKeyValueStore persistence, ITeamcityBuildApi teamcityBuildApi)
        {
            m_CommandParser = commandParser;
            m_Persistence = new ListPersistenceApi<TrackedTeamcityBuild>(persistence);
            m_TeamcityBuildApi = teamcityBuildApi;
            m_TeamcityBuildCompareEngine = new TeamcityBuildCompareEngine(m_Persistence);
        }

        public MessageResult ProcessTimerTick()
        {
            var trackedTickets = m_Persistence.ReadList(c_PersistenceKey);

            var comparison = trackedTickets.Select(x =>
                new Update<TeamcityBuildStatus>(x.Channel, x.Build, m_TeamcityBuildApi.GetBuild(x.Build.Id).Result)
            ).Where(x => x.NewValue.IsNotDefault());

            var responses = m_TeamcityBuildCompareEngine.CompareBuildStates(comparison);

            return new MessageResult(responses.ToList());
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_TeamcityBuildRegex.IsMatch(toTrack))
            {
                var build = m_TeamcityBuildApi.GetBuild(toTrack.Substring(6)).Result;
                m_Persistence.AddToList(c_PersistenceKey, new TrackedTeamcityBuild(build, message.Channel));
                return new MessageResult(new[] { Response.ToMessage(message, FormatNowTrackingMessage(toTrack)) });
            }
            return MessageResult.Empty;
        }

        private static string FormatNowTrackingMessage(string toTrack)
        {
            return string.Format("Now tracking {0}. To stop tracking, use `scbot untrack {0}`", toTrack);
        }
    }
}
