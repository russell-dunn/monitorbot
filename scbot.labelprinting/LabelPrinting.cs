using scbot.core.bot;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using scbot.github.services;
using scbot.review.services;

namespace scbot.labelprinting
{
    public class LabelPrinting : ICommandProcessor
    {
        public static IFeature Create(ICommandParser commandParser, IWebClient webClient, Configuration configuration)
        {
            string printingApiUrl = configuration.Get("printer-api-url");
            var githubApi = new GithubPRApi(webClient, configuration.Get("github-token"));
            var printer = new LabelPrinter(printingApiUrl, webClient);
            return new BasicFeature("labelprinting", 
                "[experimental] turn imaginary things into physical souvenirs to print out and keep",
                "use `print label for repo#34` to print a label for a pull request",
                new HandlesCommands(commandParser, new LabelPrinting(configuration.Get("github-default-user"), githubApi, printer)));
        }

        private readonly string defaultGithubUser;
        private readonly IGithubPRApi githubApi;
        private readonly RegexCommandMessageProcessor underlying;
        private readonly ILabelPrinter labelPrinter;

        public LabelPrinting(string defaultGithubUser, IGithubPRApi githubApi, ILabelPrinter labelPrinter) 
        {
            this.defaultGithubUser = defaultGithubUser;
            this.githubApi = githubApi;
            this.labelPrinter = labelPrinter;
            this.underlying = new RegexCommandMessageProcessor(Commands);
        }

        public Dictionary<string, MessageHandler> Commands
        {
            get
            {
                return new Dictionary<string, MessageHandler>
                {
                    { @"print label for (?<thingToPrint>.*)", PrintLabel }
                };
            }
        }

        public MessageResult ProcessCommand(Command command)
        {
            return underlying.ProcessCommand(command);
        }

        public MessageResult ProcessTimerTick()
        {
            return underlying.ProcessTimerTick();
        }

        private MessageResult PrintLabel(Command command, Match args)
        {
            var thingToPrint = args.Group("thingToPrint");
            var githubRef = GithubReferenceParser.Parse(thingToPrint);
            var user = githubRef.User ?? defaultGithubUser;
            var repo = githubRef.Repo;
            var prNum = githubRef.Issue;
            var pr = githubApi.PullRequest(user, repo, prNum).Result;

            var title = string.Format("#{0}: {1}", prNum, pr.title);
            var avatarUrl = pr.user.avatar_url;
            var images = new List<string>
            {
                "https://assets-cdn.github.com/images/modules/logos_page/GitHub-Mark.png",
                avatarUrl,
                string.Format("https://api.qrserver.com/v1/create-qr-code/?data=https://github.com/{0}/{1}/pull/{2}", user, repo, prNum),
            };

            var response = labelPrinter.PrintLabel(title, images);

            return Response.ToMessage(command, response.Substring(0, Math.Min(response.Length, 50)));
        }
    }
}
