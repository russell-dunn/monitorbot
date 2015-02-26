using System.Collections.Generic;
using System.Linq;
using scbot.bot;
using scbot.services.compareengine;
using scbot.teamcity.webhooks;
using scbot.teamcity.webhooks.tests;

namespace scbot.services.teamcity
{
    internal class TeamcityEventHandler
    {
        public IEnumerable<Response> GetResponseTo(TeamcityEvent teamcityEvent, List<Tracked<Build>> trackedBuilds, List<Tracked<Branch>> trackedBranches)
        {
            var result = new List<Response>();
            foreach (var trackedBranch in trackedBranches.Where(x => x.Value.Name == teamcityEvent.BranchName))
            {
                if (trackedBranch.Value.TrackedEventTypes.HasFlag(TeamcityEventTypes.BreakingBuilds) &&
                    teamcityEvent.BuildResultDelta == "broken")
                {
                    result.Add(new Response(string.Format("Build {0} has broken on branch {1}!", teamcityEvent.BuildName, teamcityEvent.BranchName), trackedBranch.Channel));
                }
            }

            if (teamcityEvent.EventType == "buildFinished" && teamcityEvent.BranchName == "spike/guitests")
            {
                result.Add(new Response(string.Format("{0} build finished", teamcityEvent.BuildName), "D03JWF44C"));
            }

            foreach (var trackedBuild in trackedBuilds.Where(x => x.Value.ID == teamcityEvent.BuildId))
            {
                result.Add(new Response(string.Format("{0} build updated: {1}", teamcityEvent.BuildName, teamcityEvent.EventType), trackedBuild.Channel));
            }
            return result;
        }
    }
}