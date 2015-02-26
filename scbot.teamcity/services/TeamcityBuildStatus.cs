namespace scbot.teamcity.services
{
    public struct TeamcityBuildStatus
    {
        public readonly string Id;
        public readonly string Name;
        public readonly BuildState State;
        public static readonly TeamcityBuildStatus Unknown = new TeamcityBuildStatus(null, null, BuildState.Unknown);

        public TeamcityBuildStatus(string id, string name, BuildState buildState)
        {
            Id = id;
            Name = name;
            State = buildState;
        }
    }
}
