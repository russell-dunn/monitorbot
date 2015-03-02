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
            var user = GetUser(build);
            if (build.revisions.revision != null)
            foreach (var revision in build.revisions.revision)
            {
                if (revision["vcs-root-instance"]["vcs-root-id"] == "GitHubParameterised")
                {
                    return new TeamcityRevisionForBuild(buildId, user, repo, revision.version);
                }
            }
            if (build["snapshot-dependencies"] != null && build["snapshot-dependencies"].build != null)
            foreach (var dependency in build["snapshot-dependencies"].build)
            {
                var revisionFromDependency = await RevisionForBuild(dependency.id.ToString());
                if (revisionFromDependency != null) return revisionFromDependency;
            }
            return null;
        }

        private string GetRepo(dynamic build)
        {
            return GetProperty(build, "github_repository");
        }

        private string GetUser(dynamic build)
        {
            return GetProperty(build, "github_user");
        }

        private static string GetProperty(dynamic build, string propertyName)
        {
            foreach (var property in build.properties.property)
            {
                if (property.name == propertyName) return property.value;
            }
            return null;
        }
    }
}
