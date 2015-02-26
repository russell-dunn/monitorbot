namespace scbot.teamcity.webhooks
{
    public interface IAcceptTeamcityEvents
    {
        void Accept(TeamcityEvent teamcityEvent);
    }
}
