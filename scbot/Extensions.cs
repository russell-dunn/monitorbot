using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot
{
    public static class Extensions
    {
        public static bool TryGetCommand(this ICommandParser parser, Message message, string expectedCommand, out string args)
        {
            args = null;
            string command;

            if (!parser.TryGetCommand(message, out command)) return false;
            if (!(command.StartsWith(expectedCommand+" ") || command.EndsWith(expectedCommand))) return false;

            args = command.Substring(expectedCommand.Length).Trim();
            return true;
        }

        public static bool IsNotDefault<T>(this T x)
        {
            return !object.Equals(x, default(T));
        }
    }
}
