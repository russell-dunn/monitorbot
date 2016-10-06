namespace monitorbot.teamcity.webhooks.endpoint
{
    public interface IAcceptTeamcityEvents
    {
        void Accept(TeamcityEvent teamcityEvent);
    }
}
