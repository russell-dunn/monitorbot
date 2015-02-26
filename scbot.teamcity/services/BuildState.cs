namespace scbot.teamcity.services
{
    public enum BuildState 
    { 
        Queued,
        Running,
        Failing,
        Succeeded,
        Failed,
        Unknown
    }
}
