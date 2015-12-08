using System;

namespace scbot.core.bot
{
    public class Bot : IBot
    {
        private readonly IMessageProcessor m_Processor;
        private readonly INewChannelProcessor m_NewChannelProcessor;

        public Bot(IMessageProcessor processor, INewChannelProcessor newChannelProcessor)
        {
            m_Processor = processor;
            m_NewChannelProcessor = newChannelProcessor;
        }

        public MessageResult Hello()
        {
            return MessageResult.Empty;
        }

        public MessageResult Unknown(string json)
        {
            return MessageResult.Empty;
        }

        public MessageResult Message(Message message)
        {
            return m_Processor.ProcessMessage(message);
        }

        public MessageResult ChannelCreated(string newChannelId, string newChannelName, string creatorId)
        {
            return m_NewChannelProcessor.ProcessNewChannel(newChannelId, newChannelName, creatorId);
        }

        public MessageResult TimerTick()
        {
            return m_Processor.ProcessTimerTick();
        }
    }
}
