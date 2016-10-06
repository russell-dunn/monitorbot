using System;
using System.Diagnostics;
using monitorbot.core.bot;

namespace monitorbot.core.meta
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