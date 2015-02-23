using fasttests.teamcity;
using scbot.services.compareengine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Helpers;

namespace scbot.processors.teamcity
{
    class TeamcityBuildCompareEngine
    {
        private services.ListPersistenceApi<Tracked<TeamcityBuildStatus>> m_Persistence;
        private const string c_PersistenceKey = "tracked-tc-builds";

        public TeamcityBuildCompareEngine(services.ListPersistenceApi<Tracked<TeamcityBuildStatus>> persistence)
        {
            m_Persistence = persistence;
        }

        internal IEnumerable<Response> CompareBuildStates(IEnumerable<Update<TeamcityBuildStatus>> comparison)
        {
            var differences = comparison.Select(x => new
            {
                differences = GetDifferenceString(x),
                update = x
            }).ToList();

            differences = differences.Where(x => x.differences != null).ToList();

            foreach (var diff in differences)
            {
                var id = diff.update.NewValue.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Value.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new Tracked<TeamcityBuildStatus>(diff.update.NewValue, diff.update.Channel));
                Console.WriteLine("Diff: \nold: {0}\nnew:{1}", Json.Encode(diff.update.OldValue), Json.Encode(diff.update.NewValue));
            }
            return differences.Select(x => new Response(x.differences.Message, x.update.Channel, x.differences.Image));
        }

        private static Response GetDifferenceString(Update<TeamcityBuildStatus> ttc)
        {
            var differences = GetDifferences(ttc).ToList();
            if (differences.Any())
            {
                var image = differences.Select(x => x.Image).FirstOrDefault(x => x.IsNotDefault());
                return new Response(string.Format("<http://teamcity/viewLog.html?buildId={0}|Build {0}> ({1}) updated: {2}",
                    ttc.NewValue.Id, ttc.NewValue.Name, string.Join(", ", differences.Select(x => x.Message))), null, image);
            }
            return null;
        }

        private static IEnumerable<Response> GetDifferences(Update<TeamcityBuildStatus> x)
        {
            if (x.OldValue.State != x.NewValue.State)
            {
                yield return FormatStateChanged(x);
            }
        }

        private static Response FormatStateChanged(Update<TeamcityBuildStatus> x)
        {
            if (x.NewValue.State == fasttests.teamcity.BuildState.Finished)
            {
                return new Response("build finished", null);
            }

            return new Response(string.Format("build state changed from {0} to {1}", x.OldValue.State, x.NewValue.State), null);
        }
    }
}
