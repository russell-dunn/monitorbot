using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.bot;

namespace monitorbot.core.utils
{
    public class HandlesCommands : IMessageProcessor
    {
        private readonly ICommandParser m_CommandParser;
        private readonly ICommandProcessor m_Underlying;

        public HandlesCommands(ICommandParser commandParser, ICommandProcessor underlying)
        {
            m_CommandParser = commandParser;
            m_Underlying = underlying;
        }

        public MessageResult ProcessMessage(Message message)
        {
            string command;
            if (m_CommandParser.TryGetCommand(message, out command))
            {
                return m_Underlying.ProcessCommand(new Command(message.Channel, message.User, command));
            }
            return MessageResult.Empty;
        }

        public MessageResult ProcessTimerTick()
        {
            return m_Underlying.ProcessTimerTick();
        }
    }
}
