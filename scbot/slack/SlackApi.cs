using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace scbot.slack
{
    public class SlackApi
    {
        private readonly string m_ApiKey;

        public SlackApi(string apiKey)
        {
            m_ApiKey = apiKey;
        }

        public async Task<SlackRealTimeMessaging> StartRtm()
        {
            var result = await GetApiResult("rtm.start");
            var wsUrl = result.url;
            return await SlackRealTimeMessaging.Connect(new Uri(wsUrl), new CancellationToken());
        }

        private async Task<dynamic> GetApiResult(string apiEndpoint)
        {
            using (var webClient = new WebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(string.Format("https://slack.com/api/{0}?token={1}", apiEndpoint, m_ApiKey));
                var result = Json.Decode(json);
                var ok = result.ok;
                if (!ok) throw new Exception("Error connecting to slack API: " + result.error);
                return result;
            }
        }
    }
}