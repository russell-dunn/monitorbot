using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.slack
{
    public class SlackMessageHandler
    {
        private readonly IBot m_Handler;

        public SlackMessageHandler(IBot handler)
        {
            m_Handler = handler;
        }

        public void Handle(dynamic message)
        {
            switch ((string)message.type)
            {
                case "hello": 
                    m_Handler.Hello();
                    break;
            }
        }
    }
}
