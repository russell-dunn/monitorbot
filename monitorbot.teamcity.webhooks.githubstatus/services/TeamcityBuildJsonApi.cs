using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.utils;

namespace monitorbot.teamcity.webhooks.githubstatus.services
{
    class TeamcityBuildJsonApi : ITeamcityBuildJsonApi
    {
        private readonly string m_TeamcityLogin;
        private readonly IWebClient m_WebClient;

        public TeamcityBuildJsonApi(IWebClient webClient, string teamcityLogin)
        {
            m_WebClient = webClient;
            m_TeamcityLogin = teamcityLogin;
        }

        public Task<dynamic> Build(string buildId)
        {
            return m_WebClient.DownloadJson(
                string.Format("http://teamcity.red-gate.com/guestAuth/app/rest/9.0/builds/id:{0}", buildId), 
                new[] { "Accept: application/json" });
        }
    }
}
