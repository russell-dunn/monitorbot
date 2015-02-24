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
        private readonly ListPersistenceApi<Tracked<TeamcityBuildStatus>> m_Persistence;
        private readonly ITeamcityBuildApi m_TeamcityBuildApi;
        private static readonly Regex s_TeamcityBuildRegex = new Regex(@"^build#(?<id>\d{5,10})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string c_PersistenceKey = "tracked-tc-builds";
        private readonly CompareEngine<TeamcityBuildStatus> m_TeamcityBuildCompareEngine;

        public TeamcityBuildTracker(scbot.slack.SlackCommandParser commandParser, scbot.services.InMemoryKeyValueStore persistence, ITeamcityBuildApi teamcityBuildApi)
        {
            m_CommandParser = commandParser;
            m_Persistence = new ListPersistenceApi<Tracked<TeamcityBuildStatus>>(persistence);
            m_TeamcityBuildApi = teamcityBuildApi;
            m_TeamcityBuildCompareEngine = new CompareEngine<TeamcityBuildStatus>(m_Persistence,
                x => string.Format("<http://teamcity/viewLog.html?buildId={0}|Build {0}> ({1}) updated:", x.Id, x.Name),
                new[] { new PropertyComparer<TeamcityBuildStatus>(x => x.OldValue.State != x.NewValue.State, x => FormatStateChanged(x))});
        }

        private Response FormatStateChanged(Update<TeamcityBuildStatus> x)
        {
            if (x.NewValue.State == fasttests.teamcity.BuildState.Finished)
            {
                return new Response("build finished", null);
            }

            return new Response(string.Format("build state changed from {0} to {1}", x.OldValue.State, x.NewValue.State), null);
        } 

        public MessageResult ProcessTimerTick()
        {
            var trackedTickets = m_Persistence.ReadList(c_PersistenceKey);

            var comparison = trackedTickets.Select(x =>
                new Update<TeamcityBuildStatus>(x.Channel, x.Value, m_TeamcityBuildApi.GetBuild(x.Value.Id).Result)
            ).Where(x => x.NewValue.IsNotDefault());

            var results = m_TeamcityBuildCompareEngine.CompareBuildStates(comparison).ToList();

            foreach (var result in results)
            {
                var id = result.NewValue.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Value.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new Tracked<TeamcityBuildStatus>(result.NewValue, result.Response.Channel));
            }

            return new MessageResult(results.Select(x => x.Response).ToList());
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_TeamcityBuildRegex.IsMatch(toTrack))
            {
                var build = m_TeamcityBuildApi.GetBuild(toTrack.Substring(6)).Result;
                m_Persistence.AddToList(c_PersistenceKey, new Tracked<TeamcityBuildStatus>(build, message.Channel));
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
