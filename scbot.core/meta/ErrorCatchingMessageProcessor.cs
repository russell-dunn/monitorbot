using System;
using scbot.core.bot;
using System.Diagnostics;

namespace scbot.core.meta
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
            Trace.TraceError(exception.ToString());
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