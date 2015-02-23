using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fasttests.teamcity
{
    public interface ITeamcityBuildApi
    {
        Task<TeamcityBuildStatus> GetBuild(string p);
    }
}
