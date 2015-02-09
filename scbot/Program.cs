using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using scbot.services;
using scbot.slack;

namespace scbot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var bot = new Bot(new HtmlTitleProcessor(new HtmlTitleParser()));
            var handler = new SlackMessageHandler(bot);
            var slack = new SlackApi(Configuration.SlackApiKey).StartRtm().Result;
            while (true)
            {
                var nextMessage = slack.Receive(new CancellationToken()).Result;
                handler.Handle(nextMessage);
            }
        }
    }
}
