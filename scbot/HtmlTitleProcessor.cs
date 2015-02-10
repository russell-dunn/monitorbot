using System.Linq;
using System.Text.RegularExpressions;
using scbot.services;

namespace scbot
{
    public class HtmlTitleProcessor : IMessageProcessor
    {
        private readonly IHtmlTitleParser m_HtmlTitleParser;
        private static readonly Regex s_SlackUrlRegex = new Regex(@"\<(.*?)(?:\|.*?)?\>", RegexOptions.Compiled);

        public HtmlTitleProcessor(IHtmlTitleParser htmlTitleParser)
        {
            m_HtmlTitleParser = htmlTitleParser;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            var urls = s_SlackUrlRegex.Matches(message.MessageText).Cast<Match>().Select(x => x.Groups[1].ToString());
            var titles = urls.Select(x => m_HtmlTitleParser.GetHtmlTitle(x)).Where(x => x != null);
            var responses = titles.Select(x => Response.ToMessage(message, x));
            return new MessageResult(responses);
        }
    }
}