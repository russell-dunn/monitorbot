using System;
using System.IO;
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
                    new NoteProcessor(new SlackCommandParser("scbot", "U03JWF43N"/*TODO*/), new NoteApi(new JsonFileKeyValueStore(new FileInfo("scbot.db.json")))),
                    new JiraBugProcessor(new JiraApi()),
                    new ZendeskTicketProcessor(ZendeskApi.Create(Configuration.RedgateId)),
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
