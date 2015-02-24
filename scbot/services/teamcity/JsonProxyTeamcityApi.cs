using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace scbot.services.teamcity
{
    /// <summary>
    /// A teamcity API based on an RG-specific cache/wrapper around teamcity's xml api
    /// </summary>
    class JsonProxyTeamcityApi : IJsonProxyTeamcityApi
    {
        private readonly string m_ApiBase;

        public JsonProxyTeamcityApi(string apiBase)
        {
            m_ApiBase = apiBase;
        }

        public async Task<dynamic> Build(string buildId)
        {
            using (var webClient = new WebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(string.Format("{0}build/{1}", m_ApiBase, buildId));
                return Json.Decode(json);
            }
        }
    }
}