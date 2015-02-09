using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot;
using scbot.slack;

namespace slowtests
{
    class LoggingBot : IBot
    {
        public MessageResult Hello()
        {
            Log("got hello message");
            return MessageResult.Empty;
        }

        public MessageResult UnknownMessage(string json)
        {
            Log("got unknown message type: "+json);
            return MessageResult.Empty;
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
