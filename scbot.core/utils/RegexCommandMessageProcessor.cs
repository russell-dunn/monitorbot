using scbot.core.bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scbot.core.utils
{
    public delegate MessageResult MessageHandler(Message message, Match args);

    public class RegexCommandMessageProcessor : IMessageProcessor
    {
        private readonly ICommandParser m_CommandParser;
        private readonly Dictionary<Regex, MessageHandler> m_Commands;

        public RegexCommandMessageProcessor(ICommandParser commandParser, 
            Dictionary<Regex, MessageHandler> commands)
        {
            m_CommandParser = commandParser;
            m_Commands = commands;
        }

        public RegexCommandMessageProcessor(ICommandParser commandParser,
            Dictionary<string, MessageHandler> commands)
            : this(commandParser, commands.ToDictionary(x => ToRegex(x.Key), x => x.Value))
        {
        }

        public RegexCommandMessageProcessor(ICommandParser commandParser, Regex regex, MessageHandler command)
            : this(commandParser, new Dictionary<Regex, MessageHandler> { { regex, command} }) 
        {
        }

        public RegexCommandMessageProcessor(ICommandParser commandParser, string regex, MessageHandler command)
            : this(commandParser, ToRegex(regex), command)
        {
        }

        private static Regex ToRegex(string x)
        {
            return new Regex(x, RegexOptions.Compiled | RegexOptions.IgnoreCase);
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
