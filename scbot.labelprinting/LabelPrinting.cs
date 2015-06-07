using scbot.core.bot;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Helpers;

namespace scbot.labelprinting
{
    public class LabelPrinting : IMessageProcessor
    {
        public static IFeature Create(ICommandParser commandParser, IWebClient webClient, string defaultGithubUser, string githubToken, string printingApiUrl)
        {
            return new BasicFeature("labelprinting", 
                "[experimental] turn imaginary things into physical souvenirs to print out and keep", "use `print label for repo#34` to print a label for a pull request", 
                new LabelPrinting(commandParser, webClient, Configuration.GithubDefaultUser, Configuration.GithubToken, Configuration.LabelPrinterApiUrl));
        }

        private readonly string defaultGithubUser;
        private readonly string githubToken;
        private readonly string printingApiUrl;
        private readonly RegexCommandMessageProcessor underlying;
        private readonly IWebClient webClient;

        public LabelPrinting(ICommandParser commandParser, IWebClient webClient, string defaultGithubUser, string githubToken, string printingApiUrl) 
        {
            this.webClient = webClient;
            this.defaultGithubUser = defaultGithubUser;
            this.githubToken = githubToken;
            this.printingApiUrl = printingApiUrl;
            this.underlying = new RegexCommandMessageProcessor(commandParser, Commands);
        }

        public Dictionary<string, MessageHandler> Commands
        {
            get
            {
                return new Dictionary<string, MessageHandler>
                {
                    { @"print label for (?<repo>[^ ]+?)#(?<num>[^ ]+)", PrintLabel }
                };
            }
        }

        public MessageResult ProcessMessage(Message message)
        {
            return underlying.ProcessMessage(message);
        }

        public MessageResult ProcessTimerTick()
        {
            return underlying.ProcessTimerTick();
        }

        private MessageResult PrintLabel(Message message, Match args)
        {
            var headers = new[]
            {
                "Authorization: token "+githubToken,
            };
            var repo = args.Group("repo");
            var prNum = args.Group("num");
            var url = string.Format("https://api.github.com/repos/{0}/{1}/pulls/{2}", defaultGithubUser, repo, prNum);
            var pr = webClient.DownloadJson(url, headers).Result;

            var title = string.Format("{0}#{1}: {2}", repo, prNum, pr.title);
            var text = pr.body;
            var images = new List<string>
            {
                "https://assets-cdn.github.com/images/modules/logos_page/GitHub-Mark.png",
                pr.user.avatar_url,
                string.Format("https://api.qrserver.com/v1/create-qr-code/?data=https://github.com/{0}/{1}/pull/{2}", defaultGithubUser, repo, prNum),
            };

            var printRequest = Json.Encode(new { title = title, text = text, images = images });

            var response = webClient.PostString(printingApiUrl, printRequest, new[] { "content-type:application/json" }).Result;

            return Response.ToMessage(message, response.Substring(0, Math.Min(response.Length, 50)));
        }
    }
}
