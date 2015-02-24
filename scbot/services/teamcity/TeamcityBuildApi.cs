using System;
using System.Threading.Tasks;

namespace scbot.services.teamcity
{
    class TeamcityBuildApi : ITeamcityBuildApi
    {
        private readonly IJsonProxyTeamcityApi m_Api;

        public TeamcityBuildApi(IJsonProxyTeamcityApi api)
        {
            m_Api = api;
        }

        public async Task<TeamcityBuildStatus> GetBuild(string buildId)
        {
            var json = await m_Api.Build(buildId);
            if (!json.Any()) return TeamcityBuildStatus.Unknown;
            return new TeamcityBuildStatus(buildId, json[0].name, GetStatus(json[0].status));
        }

        private BuildState GetStatus(string status)
        {
            switch (status)
            {
                case "SUCCESS": return BuildState.Succeeded;
                case "RUNNING": return BuildState.Running;
                case "FAILING": return BuildState.Failing;
                case "FAILED": return BuildState.Failed;
                case "QUEUED": return BuildState.Queued;
                default: throw new ArgumentOutOfRangeException("status", "unrecognized build status " + status);
            }
        }
    }
}