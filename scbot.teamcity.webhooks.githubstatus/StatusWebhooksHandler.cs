using scbot.core.utils;
using scbot.teamcity.webhooks.githubstatus.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.teamcity.webhooks.githubstatus
{
    public class StatusWebhooksHandler : IAcceptTeamcityEvents
    {
        private readonly IGithubStatusApi m_StatusApi;
        private readonly ITeamcityChangesApi m_TeamcityApi;

        internal StatusWebhooksHandler(IGithubStatusApi statusApi, ITeamcityChangesApi teamcityApi)
        {
            m_StatusApi = statusApi;
            m_TeamcityApi = teamcityApi;
        }

        public static StatusWebhooksHandler Create(ITime time, IWebClient webClient)
        {
            return new StatusWebhooksHandler(
                new FakeGithubStatusApi(), 
                new TeamcityChangesApi(time, new TeamcityBuildJsonApi(webClient, null)));
        }

        public void Accept(TeamcityEvent teamcityEvent)
        {
            if (teamcityEvent.BranchName == "master")
            {
                // github only shows status for comparisons, so we're not going to see any on the master branch
                return;
            }
            var revision = m_TeamcityApi.RevisionForBuild(teamcityEvent.BuildId).Result;

            var buildLink = string.Format("http://buildserver/viewLog.html?buildId={0}", teamcityEvent.BuildId);
            if (teamcityEvent.EventType == "buildStarted")
            {
                m_StatusApi.SetStatus(revision.User, revision.Repo, revision.Hash, "pending",
                    "build started", teamcityEvent.BuildName, buildLink);
            }
        }
    }
}
