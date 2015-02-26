using System.Collections.Generic;
using scbot.bot;

namespace scbot.services.teamcity
{
    internal class TeamcityEventHandler
    {
        public TeamcityEventHandler()
        {
        }

        public IEnumerable<Response> GetResponseTo(TeamcityEvent teamcityEvent)
        {
            var result = new List<Response>();
            if (teamcityEvent.BuildResultDelta == "broken" && teamcityEvent.BranchName == "master")
            {
                result.Add(new Response(string.Format("{0}: Build {1} broke on master!", teamcityEvent.EventType, teamcityEvent.BuildName), "D03JWF44C"));
            }

            if (teamcityEvent.EventType == "buildFinished" && teamcityEvent.BranchName == "spike/guitests")
            {
                result.Add(new Response(string.Format("{0} build finished", teamcityEvent.BuildName), "D03JWF44C"));
            }
            return result;
        }
    }
}