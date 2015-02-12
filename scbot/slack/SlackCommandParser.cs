using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scbot.slack
{
    public class SlackCommandParser : ICommandParser
    {
        private readonly string m_BotName;
        private readonly string m_BotUserId;
        private readonly Regex m_PingRegex;

        public SlackCommandParser(string botName, string botUserId)
        {
            m_BotName = botName.ToLowerInvariant();
            m_BotUserId = botUserId;
            m_PingRegex = new Regex(string.Format(@"
            
^ # start of line
\s* # whitespace
({0}|\<\@{1}\>)\s*:?\s* # bot name with optional colon
(?<command>.*?) # command text
\s*$ # end of line

", botName, botUserId), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
        }

        public bool TryGetCommand(Message message, out string command)
        {
            var match = m_PingRegex.Match(message.MessageText);

            if (match.Success)
            {
                command = match.Groups["command"].ToString();
                return true;
            }

            if (message.Channel.StartsWith("D"))
            {
                command = message.MessageText.Trim();
                return true;
            }

            command = null;
            return false;
        }
    }
}
