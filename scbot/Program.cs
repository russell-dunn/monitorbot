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
            var processor = new ConcattingMessageProcessor(
                new CompositeMessageProcessor(
                    new JiraBugProcessor(new JiraApi()),
                    new HtmlTitleProcessor(new HtmlTitleParser())));
            var bot = new Bot(processor);
            var slackApi = new SlackApi(Configuration.SlackApiKey);
            var slackRtm = slackApi.StartRtm().Result;
            var handler = new SlackMessageHandler(bot, slackRtm.BotId);
            var cancellationToken = new CancellationToken();
            var slackMessageEncoder = new SlackMessageEncoder();
            while (true)
            {
                var nextMessage = slackRtm.Receive(cancellationToken).Result;
                var result = handler.Handle(nextMessage);
                foreach (var response in result.Responses)
                {
                    slackApi.PostMessage(response).Wait(cancellationToken);
                    //slackRtm.Send(slackMessageEncoder.ToJSON(response), cancellationToken).Wait(cancellationToken);
                }
            }
        }
    }
}
