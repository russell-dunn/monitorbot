namespace scbot.bot
{
    class Bot : IBot
    {
        private readonly IMessageProcessor m_Processor;

        public Bot(IMessageProcessor processor)
        {
            m_Processor = processor;
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

        public MessageResult TimerTick()
        {
            return m_Processor.ProcessTimerTick();
        }
    }
}
