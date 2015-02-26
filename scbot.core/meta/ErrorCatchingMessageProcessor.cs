using System;
using scbot.bot;

namespace scbot.processors.meta
{
    public class ErrorCatchingMessageProcessor : IMessageProcessor
    {
        private readonly IMessageProcessor m_Underlying;

        public ErrorCatchingMessageProcessor(IMessageProcessor underlying)
        {
            m_Underlying = underlying;
        }

        public MessageResult ProcessTimerTick()
        {
            try
            {
                return m_Underlying.ProcessTimerTick();
            }
            catch (Exception e)
            {
                LogException(e);
                return MessageResult.Empty;
            }
        }

        private void LogException(Exception exception)
        {
            Console.WriteLine("\n\n"+exception+"\n\n");
        }

        public MessageResult ProcessMessage(Message message)
        {
            try
            {
                return m_Underlying.ProcessMessage(message);
            }
            catch (Exception e)
            {
                LogException(e);
                return MessageResult.Empty;
            }
        }
    }
}