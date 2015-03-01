using scbot.core.bot;
using scbot.core.utils;
using scbot.review.reviewer;
using scbot.review.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scbot.review
{
    public static class ReviewFactory
    {
        public static IMessageProcessor GetProcessor(ICommandParser commandParser, IWebClient webClient, 
            string githubToken, string defaultUser, string defaultRepo)
        {
            var githubApi = new GithubDiffApi(webClient, githubToken);
            var reviewApi = new ReviewApi(githubApi, 
                new Func<IDiffReviewer>[] {
                    () => new DontAddTabCharacters(),
                    () => new UseWindowsNewlines(),
                    () => new GeneralBadThing("todo", "TODO found", RegexOptions.IgnoreCase),
                    () => new GeneralBadThing("NotImplementedException", "NotImplementedException found"),
                    () => new GeneralBadThing("NotSupportedException", "NotSupportedException found"),
                    () => new GeneralBadThing(@"<SpecificVersion>\s*[fF]alse\s*</SpecificVersion>", "SpecificVersion=False found"),
                    () => new AvoidAppConfig(),
            });
            return new GithubReviewMessageProcessor(commandParser, reviewApi, defaultUser, defaultRepo);
        }
    }
}
