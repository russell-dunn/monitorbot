using System.Threading.Tasks;

namespace scbot.teamcity.services
{
    public interface IJsonProxyTeamcityApi
    {
        Task<dynamic> Build(string buildId);
    }
}