using System.Threading.Tasks;

namespace scbot.teamcity.services
{
    public interface ITeamcityBuildApi
    {
        Task<TeamcityBuildStatus> GetBuild(string buildId);
    }
}
