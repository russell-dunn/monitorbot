using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Helpers;

namespace scbot.processors.teamcity
{
    class TeamcityBuildCompareEngine
    {
        private services.ListPersistenceApi<TrackedTeamcityBuild> m_Persistence;
        private const string c_PersistenceKey = "tracked-tc-builds";

        public TeamcityBuildCompareEngine(services.ListPersistenceApi<TrackedTeamcityBuild> persistence)
        {
            m_Persistence = persistence;
        }

        internal IEnumerable<Response> CompareBuildStates(IEnumerable<TrackedTeamcityBuildComparison> comparison)
        {
            var differences = comparison.Select(x => new
            {
                differences = GetDifferenceString(x),
                comparison = x
            }).ToList();

            differences = differences.Where(x => x.differences != null).ToList();

            foreach (var diff in differences)
            {
                var id = diff.comparison.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.BuildStatus.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new TrackedTeamcityBuild(diff.comparison.NewValue, diff.comparison.Channel));
                Console.WriteLine("Diff: \nold: {0}\nnew:{1}", Json.Encode(diff.comparison.OldValue), Json.Encode(diff.comparison.NewValue));
            }
            return differences.Select(x => new Response(x.differences.Message, x.comparison.Channel, x.differences.Image));
        }

        private static Response GetDifferenceString(TrackedTeamcityBuildComparison ttc)
        {
            var differences = GetDifferences(ttc).ToList();
            if (differences.Any())
            {
                var image = differences.Select(x => x.Image).FirstOrDefault(x => x.IsNotDefault());
                return new Response(string.Format("<http://teamcity/viewLog.html?buildId={0}|Build {0}> ({1}) updated: {2}",
                    ttc.Id, ttc.NewValue.BuildName, string.Join(", ", differences.Select(x => x.Message))), null, image);
            }
            return null;
        }

        private static IEnumerable<Response> GetDifferences(TrackedTeamcityBuildComparison x)
        {
            if (x.OldValue.BuildState != x.NewValue.BuildState)
            {
                yield return FormatStateChanged(x);
            }
        }

        private static Response FormatStateChanged(TrackedTeamcityBuildComparison x)
        {
            if (x.NewValue.BuildState == fasttests.teamcity.BuildState.Finished)
            {
                return new Response("build finished", null);
            }

            return new Response(string.Format("build state changed from {0} to {1}", x.OldValue.BuildState, x.NewValue.BuildState), null);
        }
    }
}
