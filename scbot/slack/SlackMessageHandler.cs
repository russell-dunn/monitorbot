using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

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
            switch ((string)message.type)
            {
                case "hello": 
                    result = m_Handler.Hello();
                    break;
                case "message":
                    if (message.reply_to != null)
                    {
                        // Sometimes slack will re-acknowledge the last message we posted
                        // if it thinks we disconected
                        result = MessageResult.Empty;
                        break;
                    }
                    var username = message.user ?? message.username;
                    if (username == m_BotId || username == "scbot")
                    {
                        result = MessageResult.Empty;
                        break;
                    }
                    result = m_Handler.Message(new Message(message.channel, username, message.text));
                    break;
                default:
                    result = m_Handler.Unknown(json);
                    break;
            }

            return result;
        }
    }
}
