using scbot.core.bot;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace scbot.silly
{
    public class Silly : IMessageProcessor
    {
        private readonly RegexCommandMessageProcessor m_Underlying;
        private readonly IWebClient m_WebClient;

        public Silly(ICommandParser parser, IWebClient webclient)
        {
            m_Underlying = new RegexCommandMessageProcessor(parser, Commands);
            m_WebClient = webclient;
        }

        public Dictionary<Regex, MessageHandler> Commands
        {
            get
            {
                return new Dictionary<Regex, MessageHandler>
                {
                    { new Regex(@"quote"), Quote },
                    { new Regex(@"class name|name (a )?class"), ClassName },
                    //{ new Regex(@"method name|name (a )?method"), MethodName },
                };
            }
        }

        private MessageResult Quote(Message message, Match args)
        {
            var categories = String.Join("+", new[] { "computers", "fortunes", "technology", "computation", "science" });
            var url = "http://www.iheartquotes.com/api/v1/random?format=json&max_lines=5&source="+categories;
            var quoteResult = m_WebClient.DownloadJson(url).Result;
            return Response.ToMessage(message, string.Format("{1}\n<{0}|[source]>", quoteResult.link, quoteResult.quote));
        }

        private MessageResult ClassName(Message message, Match args)
        {
            var className = m_WebClient.DownloadString("http://www.classnamer.com/index.txt?generator=spring").Result;
            return Response.ToMessage(message, string.Format("How about {0}?", className));
        }

        public MessageResult ProcessMessage(Message message)
        {
            return ((IMessageProcessor)m_Underlying).ProcessMessage(message);
        }

        public MessageResult ProcessTimerTick()
        {
            return ((IMessageProcessor)m_Underlying).ProcessTimerTick();
        }
    }
}
