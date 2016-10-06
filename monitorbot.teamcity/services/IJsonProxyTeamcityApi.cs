using System.Threading.Tasks;

namespace monitorbot.teamcity.services
{
    public interface IJsonProxyTeamcityApi
    {
        Task<dynamic> Build(string buildId);
    }
}