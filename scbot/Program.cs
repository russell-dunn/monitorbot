using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using scbot.processors;
using scbot.services;
using scbot.slack;

namespace scbot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var htmlDomainBlacklist = new[] {"jira", "jira.red-gate.com", "rg-jira01", "rg-jira01.red-gate.com", "redgatesupport.zendesk.com"};
            var processor = new ConcattingMessageProcessor(
                new CompositeMessageProcessor(
                    new JiraBugProcessor(new JiraApi()),
                    new HtmlTitleProcessor(new HtmlTitleParser(), htmlDomainBlacklist)));
            var bot = new Bot(processor);
            var slackApi = new SlackApi(Configuration.SlackApiKey);
            var slackRtm = slackApi.StartRtm().Result;
            var handler = new SlackMessageHandler(bot, slackRtm.BotId);
            var cancellationToken = new CancellationToken();
            while (true)
            {
                var nextMessage = slackRtm.Receive(cancellationToken).Result;
                var result = handler.Handle(nextMessage);
                foreach (var response in result.Responses)
                {
                    slackApi.PostMessage(response).Wait(cancellationToken);
                }
            }
        }
    }
}
