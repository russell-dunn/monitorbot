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
        public static IFeature Create(ICommandParser commandParser, IWebClient webClient, 
            Configuration configuration)
        {
            var githubApi = new GithubDiffApi(webClient, configuration.GithubToken);
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
            var processor = new GithubReviewMessageProcessor(commandParser, reviewApi, configuration.GithubDefaultUser, configuration.GithubDefaultRepo);
            return new BasicFeature("githubreview", 
                "[experimental] run some automated checks against github pull requests", "- `review fooRepo#123` review a pull request\n- `review fooRepo@bug/SC-1234` review a branch (against master)\n- `review fooCorp/fooRepo@abc123` review a specific commit", 
                processor);
        }
    }
}
