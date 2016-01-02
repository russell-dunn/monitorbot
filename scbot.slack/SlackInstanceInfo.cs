using System.Collections.Generic;

namespace scbot.slack
{
    public class SlackInstanceInfo
    {
        public SlackInstanceInfo(string botId, IEnumerable<SlackUser> users)
        {
            BotId = botId;
            Users = users;
        }

        public string BotId { get; }
        public IEnumerable<SlackUser> Users { get; }
    }
}