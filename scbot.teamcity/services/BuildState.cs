namespace scbot.services.teamcity
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
