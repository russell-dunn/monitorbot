using monitorbot.teamcity.webhooks.tests;

namespace monitorbot.teamcity.webhooks
{
    struct Branch
    {
        public readonly TeamcityEventTypes TrackedEventTypes;
        public readonly string Name;
        public readonly string Repo;

        public Branch(TeamcityEventTypes trackedEventTypes, string name, string repo = null)
        {
            TrackedEventTypes = trackedEventTypes;
            Name = name;
            Repo = repo;
        }
    }
}
