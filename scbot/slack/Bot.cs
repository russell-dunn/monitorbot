using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.slack
{
    class Bot : IBot
    {
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
            return MessageResult.Empty;
        }
    }
}
