using scbot.core.utils;
using scbot.teamcity.webhooks.githubstatus.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.teamcity.webhooks.endpoint;

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

        public static StatusWebhooksHandler Create(ITime time, IWebClient webClient, string githubToken)
        {
            return new StatusWebhooksHandler(
                new GithubStatusApi(webClient, githubToken), 
                new TeamcityChangesApi(time, new TeamcityBuildJsonApi(webClient, null)));
        }

        public void Accept(TeamcityEvent teamcityEvent)
        {
            if (teamcityEvent.BranchName == "master")
            {
                // github only shows status for comparisons, so we're not going to see any on the master branch
                return;
            }
            var buildId = teamcityEvent.BuildId;
            var revision = m_TeamcityApi.RevisionForBuild(buildId).Result;

            if (revision == null)
            {
                // build doesn't have a revision yet -- try again soon
                Console.WriteLine("Delaying revision fetch for build #" + buildId);
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    Console.WriteLine("Retrying for build #"+buildId);
                    Accept(teamcityEvent);
                });
                return;
            }

            SetStatus(teamcityEvent, revision);
        }

        private void SetStatus(TeamcityEvent teamcityEvent, TeamcityRevisionForBuild revision)
        {
            var buildLink = string.Format("http://buildserver/viewLog.html?buildId={0}", teamcityEvent.BuildId);
            var description = AbbreviateBuildName(teamcityEvent);
            switch (teamcityEvent.EventType)
            {
                case TeamcityEventType.BuildStarted:
                    m_StatusApi.SetStatus(revision.User, revision.Repo, revision.Hash, "pending", "build started", description, buildLink);
                    break;
                case TeamcityEventType.BuildFinished:
                    var status = teamcityEvent.BuildState == TeamcityBuildState.Success ? "success" : "failure";
                    m_StatusApi.SetStatus(revision.User, revision.Repo, revision.Hash, status, teamcityEvent.BuildStateText, description, buildLink);
                    break;
            }
        }

        private static string AbbreviateBuildName(TeamcityEvent teamcityEvent)
        {
            return teamcityEvent.BuildName
                .Replace("SQL Compare Engine", "SCE")
                .Replace("SQL Data Compare Engine", "SDCE")
                .Replace("SQL Compare UI", "SCUI")
                .Replace("SQL Data Compare UI", "SDCUI")
                .Replace("SQL Compare", "SC")
                .Replace("SQL Data Compare", "SDC");
        }
    }
}
