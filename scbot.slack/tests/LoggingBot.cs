using System;
using System.Linq;
using scbot.core.bot;

namespace scbot.slack.tests
{
    class LoggingBot : IBot
    {
        public MessageResult Hello()
        {
            Log("got hello message");
            return MessageResult.Empty;
        }

        public MessageResult Unknown(string json)
        {
            Log("got unknown message type: "+json);
            return MessageResult.Empty;
        }

        public MessageResult Message(Message message)
        {
            Log("got message from channel {0} user {1} with text {2}", message.Channel, message.User, message.MessageText);
            return MessageResult.Empty;
        }

        public MessageResult TimerTick()
        {
            return MessageResult.Empty;
        }

        private static void Log(string message, params object[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message, args);
            }
        }
    }
}
