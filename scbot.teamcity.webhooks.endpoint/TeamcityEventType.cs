namespace scbot.teamcity.webhooks
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