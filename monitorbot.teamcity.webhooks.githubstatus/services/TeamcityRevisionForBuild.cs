using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.teamcity.webhooks.githubstatus.services
{
    class TeamcityRevisionForBuild
    {
        public readonly string BuildId;
        public readonly string Hash;
        public readonly string Repo;
        public readonly string User;

        public TeamcityRevisionForBuild(string buildId, string user, string repo, string hash)
        {
            BuildId = buildId;
            User = user;
            Repo = repo;
            Hash = hash;
        }
    }
}
