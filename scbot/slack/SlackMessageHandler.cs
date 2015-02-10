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

        public SlackMessageHandler(IBot handler)
        {
            m_Handler = handler;
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
                    result = m_Handler.Message(new Message(message.channel, message.user, message.text));
                    break;
                default:
                    result = m_Handler.Unknown(json);
                    break;
            }

            return result;
        }
    }
}
