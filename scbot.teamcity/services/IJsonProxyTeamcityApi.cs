using System.Threading.Tasks;

namespace scbot.services.teamcity
{
    public interface IJsonProxyTeamcityApi
    {
        Task<dynamic> Build(string buildId);
    }
}