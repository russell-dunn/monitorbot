namespace scbot.services.teamcity
{
    public struct TeamcityBuildStatus
    {
        public readonly string Id;
        public readonly string Name;
        public readonly BuildState State;

        public TeamcityBuildStatus(string id, string name, BuildState buildState)
        {
            Id = id;
            Name = name;
            State = buildState;
        }
    }
}
