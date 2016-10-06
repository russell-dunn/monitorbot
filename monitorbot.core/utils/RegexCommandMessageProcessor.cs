using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using monitorbot.core.bot;

namespace monitorbot.core.utils
{
    public delegate MessageResult MessageHandler(Command command, Match args);

    public class RegexCommandMessageProcessor : ICommandProcessor
    {
        private readonly Dictionary<Regex, MessageHandler> m_Commands;

        public RegexCommandMessageProcessor(Dictionary<Regex, MessageHandler> commands)
        {
            m_Commands = commands;
        }

        public RegexCommandMessageProcessor(Dictionary<string, MessageHandler> commands)
            : this(commands.ToDictionary(x => ToRegex(x.Key), x => x.Value))
        {
        }

        public RegexCommandMessageProcessor(Regex regex, MessageHandler command)
            : this(new Dictionary<Regex, MessageHandler> { { regex, command} }) 
        {
        }

        public RegexCommandMessageProcessor(string regex, MessageHandler command)
            : this(ToRegex(regex), command)
        {
        }

        private static Regex ToRegex(string x)
        {
            return new Regex(x, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public MessageResult ProcessCommand(Command incomingCommand)
        {
            foreach (var command in m_Commands)
            {
                Match match;
                if (command.Key.TryMatch(incomingCommand.CommandText, out match))
                {
                    return command.Value(incomingCommand, match);
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
