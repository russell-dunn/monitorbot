using System.Threading.Tasks;

namespace scbot.services.teamcity
{
    internal interface IJsonProxyTeamcityApi
    {
        Task<dynamic> Build(string buildId);
    }
}