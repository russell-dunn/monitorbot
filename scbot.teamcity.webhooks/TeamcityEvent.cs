namespace scbot.services.teamcity
{
    public class TeamcityEvent
    {
        public readonly string EventType;
        public readonly string BuildId;
        public readonly string BuildTypeId;
        public readonly string BuildName;
        public readonly string BuildResultDelta;
        public readonly string BranchName;

        public TeamcityEvent(string eventType, string buildId, string buildTypeId, string buildName, string buildResultDelta, string branchName)
        {
            EventType = eventType;
            BuildId = buildId;
            BuildTypeId = buildTypeId;
            BuildName = buildName;
            BuildResultDelta = buildResultDelta;
            BranchName = branchName;
        }
    }
}