using System.Threading.Tasks;

namespace monitorbot.teamcity.services
{
    public interface ITeamcityBuildApi
    {
        Task<TeamcityBuildStatus> GetBuild(string buildId);
    }
}
