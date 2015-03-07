using scbot.core.bot;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scbot.rg
{
    public class RegexCommandMessageProcessor : IMessageProcessor
    {
        private readonly ICommandParser m_CommandParser;
        private readonly Dictionary<Regex, Func<Message, Match, MessageResult>> m_Commands;

        public RegexCommandMessageProcessor(ICommandParser commandParser, 
            Dictionary<Regex, Func<Message, Match, MessageResult>> commands)
        {
            m_CommandParser = commandParser;
            m_Commands = commands;
        }

        public RegexCommandMessageProcessor(ICommandParser commandParser, Regex regex, Func<Message, Match, MessageResult> command)
            : this(commandParser, new Dictionary<Regex, Func<Message, Match, MessageResult>> { { regex, command} }) 
        {
        }

        public MessageResult ProcessMessage(Message message)
        {
            foreach (var command in m_Commands)
            {
                string commandText;
                Match match;
                if (m_CommandParser.TryGetCommand(message, out commandText) && 
                    command.Key.TryMatch(commandText, out match))
                {
                    return command.Value(message, match);
                }
            }
            return MessageResult.Empty;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }
    }
}
