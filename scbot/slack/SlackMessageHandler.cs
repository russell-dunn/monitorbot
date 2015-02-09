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

        public void Handle(string json)
        {
            var message = Json.Decode(json);
            switch ((string)message.type)
            {
                case "hello": 
                    m_Handler.Hello();
                    break;
                default:
                    m_Handler.UnknownMessage(json);
                    break;
            }
        }
    }
}
