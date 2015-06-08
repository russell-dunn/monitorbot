using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.utils
{
    public class Command
    {
        public readonly string Channel;
        public readonly string User;
        public readonly string CommandText;

        public Command(string channel, string user, string commandText)
        {
            Channel = channel;
            User = user;
            CommandText = commandText;
        }

        public bool TryGetArgs(string expectedCommand, out string args)
        {
            args = null;

            if (!(CommandText.StartsWith(expectedCommand + " ") || CommandText.EndsWith(expectedCommand)))
            {
                return false;
            }

            args = CommandText.Substring(expectedCommand.Length).Trim();
            return true;
        }
    }
}
