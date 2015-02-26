using System;
using System.Web.Helpers;
using scbot.core.bot;

namespace scbot.slack
{
    public class SlackMessageHandler
    {
        private readonly IBot m_Handler;
        private readonly string m_BotId;

        public SlackMessageHandler(IBot handler, string botId)
        {
            m_Handler = handler;
            m_BotId = botId;
        }

        public MessageResult Handle(string json)
        {
            var message = Json.Decode(json);
            MessageResult result;

            if (message.error != null)
            {
                throw new InvalidOperationException("Error from slack: " + message.error.message);
            }

            switch ((string)message.type)
            {
                case "hello": 
                    result = m_Handler.Hello();
                    break;
                case "message":
                    // Sometimes slack will re-acknowledge the last message we posted
                    // if it thinks we disconected
                    var replyTo = message.reply_to;
                    var userId = message.user;
                    var subtype = message.subtype;
                    var hidden = message.hidden;
                    var text = message.text;
                    if (userId == m_BotId || subtype == "bot_message" || subtype == "file_share" || replyTo != null || hidden != null || text == null)
                    {
                        result = MessageResult.Empty;
                        break;
                    }
                    result = m_Handler.Message(new Message(message.channel, userId, text));
                    break;
                default:
                    result = m_Handler.Unknown(json);
                    break;
            }

            return result;
        }

        public MessageResult HandleTimerTick()
        {
            return m_Handler.TimerTick();
        }
    }
}
