using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using monitorbot.core.bot;
using monitorbot.core.utils;
using monitorbot.review.reviewer;
using monitorbot.review.services;

namespace monitorbot.review
{
    public static class ReviewFactory
    {
        public static IFeature Create(ICommandParser commandParser, IWebClient webClient, 
            Configuration configuration)
        {
            var githubApi = new GithubDiffApi(webClient, configuration.Get("github-token"));
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
            var processor = new GithubReviewMessageProcessor(commandParser, reviewApi, configuration.Get("github-default-user"), configuration.Get("github-default-repo"));
            return new BasicFeature("githubreview", 
                "[experimental] run some automated checks against github pull requests", "- `review fooRepo#123` review a pull request\n- `review fooRepo@bug/SC-1234` review a branch (against master)\n- `review fooCorp/fooRepo@abc123` review a specific commit", 
                processor);
        }
    }
}
