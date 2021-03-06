﻿using System;
using System.Linq;
using System.Diagnostics;
using monitorbot.core.bot;

namespace monitorbot.slack.tests
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
                Trace.WriteLine(message);
            }
            else
            {
                Trace.WriteLine(string.Format(message, args));
            }
        }

        public MessageResult ChannelCreated(string newChannelId, string newChannelName, string creatorId)
        {
            Log("got new channel message for channel {0} ({1}) created by {2}", newChannelId, newChannelName, creatorId);
            return MessageResult.Empty;
        }
    }
}
