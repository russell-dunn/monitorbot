using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scbot.review.services
{
    static class GithubReferenceParser
    {
        public static GithubReference Parse(string input)
        {
            if (input.Trim().Contains(" "))
            {
                return null;
            }

            if (input.Contains("#"))
            {
                var hashSplit = input.Split(new[] { '#' }, 2);
                var beforeHash = hashSplit[0];
                var afterHash = hashSplit[1];
                int issue;
                if (!int.TryParse(afterHash, out issue))
                {
                    return null;
                }

                if (beforeHash.Contains("/"))
                {
                    var slashSplit = beforeHash.Split(new[] { '/' }, 2);
                    return GithubReference.FromIssue(slashSplit[0], slashSplit[1], issue);
                }

                if (String.IsNullOrWhiteSpace(beforeHash))
                {
                    beforeHash = null;
                }

                return GithubReference.FromIssue(null, beforeHash, issue);
            }

            if (input.Contains("@"))
            {
                var atSplit = input.Split(new[] { '@' }, 2);
                var beforeAt = atSplit[0];
                var afterAt = atSplit[1];

                if (beforeAt.Contains("/"))
                {
                    var slashSplit = beforeAt.Split(new[] { '/' }, 2);
                    return ParseCommitOrBranch(slashSplit[0], slashSplit[1], afterAt);
                }

                return ParseCommitOrBranch(null, beforeAt, afterAt);
            }

            return ParseCommitOrBranch(null, null, input);
        }

        private static GithubReference ParseCommitOrBranch(string user, string repo, string commitOrBranch)
        {
            string[] comparison;
            if (TrySplitComparison(commitOrBranch, out comparison) && comparison.Length == 2)
            {
                return GithubReference.FromComparison(user, repo, comparison[1], comparison[0]);
            }

            if (Regex.IsMatch(commitOrBranch, @"^[a-f0-9]{4,40}$"))
            {
                return GithubReference.FromCommit(user, repo, commitOrBranch);
            }

            return GithubReference.FromBranch(user, repo, commitOrBranch);
        }

        private static bool TrySplitComparison(string input, out string[] result)
        {
            if (input.Contains("..."))
            {
                result = input.Split(new string[] { "..." }, StringSplitOptions.None);
                return true;
            }
            if (input.Contains(".."))
            {
                result = input.Split(new string[] { ".." }, StringSplitOptions.None);
                return true;
            }
            result = null;
            return false;
        }
    }
}
