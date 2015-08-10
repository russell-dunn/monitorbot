using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.core.utils;

namespace scbot.github.services
{
    public class GithubPRApi : IGithubPRApi
    {
        private readonly string m_GithubToken;
        private readonly IWebClient m_WebClient;

        public GithubPRApi(IWebClient webClient, string githubToken)
        {
            m_WebClient = webClient;
            m_GithubToken = githubToken;
        }

        private async Task<dynamic> Get(string user, string repo, string api)
        {
            var url = string.Format("https://api.github.com/repos/{0}/{1}/{2}", user, repo, api);
            var headers = new[]
            {
                "Authorization: token "+m_GithubToken,
            };
            return await m_WebClient.DownloadJson(url, headers);
        }

        public Task<dynamic> PullRequest(string user, string repo, int prNum)
        {
            return Get(user, repo, "pulls/" + prNum);
        }
    }
}
