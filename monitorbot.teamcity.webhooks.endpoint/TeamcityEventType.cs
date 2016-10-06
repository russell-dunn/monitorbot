namespace monitorbot.teamcity.webhooks.endpoint
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