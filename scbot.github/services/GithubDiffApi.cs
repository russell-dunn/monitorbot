using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.core.utils;

namespace scbot.review.services
{
    public class GithubDiffApi : IGithubDiffApi
    {
        private readonly string m_GithubToken;
        private readonly IWebClient m_WebClient;

        public GithubDiffApi(IWebClient webClient, string githubToken)
        {
            m_WebClient = webClient;
            m_GithubToken = githubToken;
        }

        private async Task<dynamic> Get(string user, string repo, string api)
        {
            var url = string.Format("https://api.github.com/repos/{0}/{1}/{2}", user, repo, api);
            var headers = new[]
            {
                "Accept: application/vnd.github.diff",
                "Authorization: token "+m_GithubToken,
            };
            return await m_WebClient.DownloadString(url, headers);
        }

        public Task<dynamic> DiffForCommit(string user, string repo, string hash)
        {
            return Get(user, repo, "commits/" + hash);
        }

        public Task<dynamic> DiffForComparison(string user, string repo, string comparison)
        {
            return Get(user, repo, "compare/" + comparison);
        }

        public Task<dynamic> DiffForPullRequest(string user, string repo, int pr)
        {
            return Get(user, repo, "pulls/" + pr);
        }
    }
}
