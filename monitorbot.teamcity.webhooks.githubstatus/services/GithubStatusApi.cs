using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using monitorbot.core.utils;

namespace monitorbot.teamcity.webhooks.githubstatus.services
{
    class GithubStatusApi : IGithubStatusApi
    {
        private readonly IWebClient m_WebClient;
        private readonly string m_GithubToken;

        public GithubStatusApi(IWebClient webClient, string githubToken)
        {
            m_WebClient = webClient;
            m_GithubToken = githubToken;
        }

        public Task SetStatus(string user, string repo, string commit, string status, string description, string context, string link)
        {
            var statusJson = Json.Encode(new
            {
                state = status,
                target_url = link,
                description = description,
                context = context
            });
            var headers = new[]
            {
                "Authorization: token "+m_GithubToken,
            };
            var url = string.Format("https://api.github.com/repos/{0}/{1}/statuses/{2}", user, repo, commit);
            return m_WebClient.PostString(url, statusJson, headers);
        }
    }
}
