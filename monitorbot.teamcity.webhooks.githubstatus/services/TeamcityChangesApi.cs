using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.utils;

namespace monitorbot.teamcity.webhooks.githubstatus.services
{
    class TeamcityChangesApi : ITeamcityChangesApi
    {
        private readonly ITeamcityBuildJsonApi m_Api;
        private readonly Cache<string, TeamcityRevisionForBuild> m_Cache;

        public TeamcityChangesApi(ITime time, ITeamcityBuildJsonApi api)
        {
            m_Api = api;
            m_Cache = new Cache<string, TeamcityRevisionForBuild>(time, TimeSpan.FromDays(1));
        }

        public async Task<TeamcityRevisionForBuild> RevisionForBuild(string buildId)
        {
            if (m_Cache.Get(buildId) != null)
            {
                return m_Cache.Get(buildId);
            }

            var build = await m_Api.Build(buildId);
            var repo = GetRepo(build);
            var user = GetUser(build);
            if (build.revisions.revision != null)
            foreach (var revision in build.revisions.revision)
            {
                if (revision["vcs-root-instance"]["vcs-root-id"] == "SqlCompareDataCompareStaging_GitHubAutocrlfParameterised")
                {
                    var result = new TeamcityRevisionForBuild(buildId, user, repo, revision.version);
                    m_Cache.Set(buildId, result);
                    return result;
                }
            }
            if (build["snapshot-dependencies"] != null && build["snapshot-dependencies"].build != null)
            foreach (var dependency in build["snapshot-dependencies"].build)
            {
                var revisionFromDependency = await RevisionForBuild(dependency.id.ToString());
                if (revisionFromDependency != null)
                {
                    return revisionFromDependency;
                }
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
            if (build.properties != null && build.properties.property != null)
            foreach (var property in build.properties.property)
            {
                if (property.name == propertyName) return property.value;
            }
            return null;
        }
    }
}
