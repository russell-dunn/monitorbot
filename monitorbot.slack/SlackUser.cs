namespace monitorbot.slack
{
    public class SlackUser
    {
        public SlackUser(string displayName, string userName, string slackId)
        {
            DisplayName = displayName;
            UserName = userName;
            SlackId = slackId;
        }

        public string DisplayName { get; }
        public string SlackId { get; }
        public string UserName { get; }
    }
}