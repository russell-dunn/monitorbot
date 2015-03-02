namespace scbot.teamcity.webhooks.endpoint
{
    public enum TeamcityEventType
    {
        BuildStarted,
        BuildInterrupted,
        BeforeBuildFinish,
        BuildFinished,
        Unknown,
    }
}