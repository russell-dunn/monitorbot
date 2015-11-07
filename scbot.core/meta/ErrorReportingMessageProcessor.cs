using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.core.bot;
using scbot.core.utils;

namespace scbot.core.meta
{
    public class ErrorReportingMessageProcessor : IMessageProcessor
    {
        private readonly IPasteBin m_Pastebin;
        private IMessageProcessor m_Underlying;

        public ErrorReportingMessageProcessor(IMessageProcessor underlying, IPasteBin pastebin)
        {
            m_Underlying = underlying;
            m_Pastebin = pastebin;
        }

        public MessageResult ProcessMessage(Message message)
        {
            try
            {
                return m_Underlying.ProcessMessage(message);
            }
            catch (Exception e)
            {
                return LogException(e, message);
            }
        }

        public MessageResult ProcessTimerTick()
        {
            try
            {
                return m_Underlying.ProcessTimerTick();
            }
            catch (Exception e)
            {
                return LogException(e);
            }
        }

        private MessageResult LogException(Exception exception, Message? incomingMessage = null)
        {
            var message = incomingMessage != null ? 
                incomingMessage.Value.Channel + " " + incomingMessage.Value.User + " " + incomingMessage.Value.MessageText :
                "tick";
            var report = string.Format("{0}: {1}\n{2}", DateTime.Now, message, exception.ToString());
            var pasteUrl = m_Pastebin.UploadPaste(report);
            var response = string.Format("DANGER WILL ROBINSON: A <{0}|{1}> was encountered while processing this request.",
                pasteUrl, exception.GetType().Name);
            Trace.TraceError(response + "\n" + exception.ToString());
            if (incomingMessage != null)
            {
                return Response.ToMessage(incomingMessage.Value, response);
            }
            return MessageResult.Empty;
        }
    }
}
