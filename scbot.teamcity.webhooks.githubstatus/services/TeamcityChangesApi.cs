using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.teamcity.webhooks.githubstatus.services
{
    class TeamcityChangesApi : ITeamcityChangesApi
    {
        private readonly ITeamcityBuildJsonApi m_Api;

        public TeamcityChangesApi(ITeamcityBuildJsonApi api)
        {
            m_Api = api;
        }

        public async Task<TeamcityRevisionForBuild> RevisionForBuild(string buildId)
        {
            var build = await m_Api.Build(buildId);
            var repo = GetRepo(build);
            if (build.revisions.revision != null)
            foreach (var revision in build.revisions.revision)
            {
                if (revision["vcs-root-instance"]["vcs-root-id"] == "GitHubParameterised")
                {
                    return new TeamcityRevisionForBuild(buildId, repo, revision.version);
                }
            }
            foreach (var dependency in build["snapshot-dependencies"].build)
            {
                var revisionFromDependency = await RevisionForBuild(dependency.id.ToString());
                if (revisionFromDependency != null) return revisionFromDependency;
            }
            return null;
        }

        private string GetRepo(dynamic build)
        {
            foreach (var property in build.properties.property)
            {
                if (property.name == "github_repository") return property.value;
            }
            return null;
        }
    }
}
