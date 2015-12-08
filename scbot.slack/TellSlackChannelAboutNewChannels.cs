using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.core.bot;

namespace scbot.slack
{
    public class TellSlackChannelAboutNewChannels : INewChannelProcessor
    {
        private readonly string m_ChannelToNotify;

        public TellSlackChannelAboutNewChannels(string channelToNotify)
        {
            m_ChannelToNotify = channelToNotify;
        }

        public MessageResult ProcessNewChannel(string newChannelId, string newChannelName, string creatorId)
        {
            return new Response(string.Format("<@{0}> created a new channel: <#{1}>", creatorId, newChannelId), m_ChannelToNotify);
        }
    }
}
