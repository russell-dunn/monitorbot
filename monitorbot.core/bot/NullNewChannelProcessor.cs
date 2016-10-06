using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.core.bot
{
    public class NullNewChannelProcessor : INewChannelProcessor
    {
        public MessageResult ProcessNewChannel(string newChannelId, string newChannelName, string creatorId)
        {
            return MessageResult.Empty;
        }
    }
}
