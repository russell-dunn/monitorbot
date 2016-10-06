using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.core.bot
{
    public interface INewChannelProcessor
    {
        MessageResult ProcessNewChannel(string newChannelId, string newChannelName, string creatorId);
    }
}
