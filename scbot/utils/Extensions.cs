using scbot.bot;

namespace scbot.utils
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

        public static bool IsDefault<T>(this T x)
        {
            return Equals(x, default(T));
        }

        public static bool IsNotDefault<T>(this T x)
        {
            return !Equals(x, default(T));
        }
    }
}
