using System.Collections.Generic;
using System.Linq;
using scbot.core.bot;
using scbot.core.compareengine;
using scbot.teamcity.webhooks.tests;

namespace scbot.teamcity.webhooks
{
    internal class TeamcityEventHandler
    {
        public IEnumerable<Response> GetResponseTo(TeamcityEvent teamcityEvent, List<Tracked<Build>> trackedBuilds, List<Tracked<Branch>> trackedBranches)
        {
            var result = new List<Response>();
            foreach (var trackedBranch in trackedBranches.Where(x => x.Value.Name == teamcityEvent.BranchName))
            {
                if (trackedBranch.IsTracking(TeamcityEventTypes.BreakingBuilds) &&
                    teamcityEvent.BuildResultDelta == "broken")
                {
                    result.Add(new Response(string.Format("Build {0} has broken on branch {1}!", teamcityEvent.BuildName, teamcityEvent.BranchName), trackedBranch.Channel));
                }
                else if (trackedBranch.IsTracking(TeamcityEventTypes.FinishedBuilds) &&
                         teamcityEvent.EventType == "buildFinished")
                {
                    result.Add(new Response(string.Format("Build {0} has finished", teamcityEvent.BuildName), trackedBranch.Channel));
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