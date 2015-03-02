using System.Text;

namespace scbot.teamcity.webhooks.endpoint
{
    public class TeamcityEvent
    {
        public readonly TeamcityEventType EventType;
        public readonly string BuildId;
        public readonly string BuildTypeId;
        public readonly string BuildName;
        public readonly BuildResultDelta BuildResultDelta;
        public readonly string BranchName;
        public readonly TeamcityBuildState BuildState;
        public readonly string BuildStateText;
        public readonly string BuildNumber;

        public TeamcityEvent(TeamcityEventType eventType, string buildId, string buildTypeId, string buildName, BuildResultDelta buildResultDelta, 
            string branchName, TeamcityBuildState buildState, string buildStateText, string buildNumber)
        {
            EventType = eventType;
            BuildId = buildId;
            BuildTypeId = buildTypeId;
            BuildName = buildName;
            BuildResultDelta = buildResultDelta;
            BranchName = branchName;
            BuildState = buildState;
            BuildStateText = buildStateText;
            BuildNumber = buildNumber;
        }
    }
}