using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.teamcity.webhooks.githubstatus.services
{
    class TeamcityRevisionForBuild
    {
        public readonly string BuildId;
        public readonly string Hash;
        public readonly string Repo;

        public TeamcityRevisionForBuild(string buildId, string repo, string hash)
        {
            BuildId = buildId;
            Repo = repo;
            Hash = hash;
        }
    }
}
