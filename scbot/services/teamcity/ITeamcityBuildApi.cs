using System.Threading.Tasks;

namespace scbot.services.teamcity
{
    public interface ITeamcityBuildApi
    {
        Task<TeamcityBuildStatus> GetBuild(string buildId);
    }
}
