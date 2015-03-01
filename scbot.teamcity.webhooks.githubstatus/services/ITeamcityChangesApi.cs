using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.teamcity.webhooks.githubstatus.services
{
    interface ITeamcityChangesApi
    {
        Task<TeamcityRevisionForBuild> RevisionForBuild(string buildId);
    }
}
