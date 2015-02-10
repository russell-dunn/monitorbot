using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using scbot.services;

namespace scbot.processors
{
    public class HtmlTitleProcessor : IMessageProcessor
    {
        private readonly IHtmlTitleParser m_HtmlTitleParser;
        private readonly HashSet<string> m_DomainBlacklist;
        private static readonly Regex s_SlackUrlRegex = new Regex(@"\<(.*?)(?:\|.*?)?\>", RegexOptions.Compiled);

        public HtmlTitleProcessor(IHtmlTitleParser htmlTitleParser, IEnumerable<string> domainBlacklist)
        {
            m_HtmlTitleParser = htmlTitleParser;
            m_DomainBlacklist = new HashSet<string>(domainBlacklist);
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            var urls = s_SlackUrlRegex.Matches(message.MessageText).Cast<Match>().Select(x => x.Groups[1].ToString());
            urls = urls.Where(x => !m_DomainBlacklist.Contains(GetDomain(x)));
            var titles = urls.Select(x => m_HtmlTitleParser.GetHtmlTitle(x)).Where(x => x != null);
            var responses = titles.Select(x => Response.ToMessage(message, x));
            return new MessageResult(responses);
        }

        private static string GetDomain(string url)
        {
            try
            {
                return new Uri(url).Host;
            }
            catch
            {
                return null;
            }
        }
    }
}