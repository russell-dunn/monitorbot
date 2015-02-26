using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using scbot.core.bot;

namespace scbot.slack
{
    public class SlackApi
    {
        private readonly string m_ApiKey;

        public SlackApi(string apiKey)
        {
            m_ApiKey = apiKey;
        }

        public async Task<ISlackRealTimeMessaging> StartRtm()
        {
            var result = await GetApiResult("rtm.start");
            var wsUrl = result.url;
            // TODO: getting the bot id here seems to be the most convenient but it's probably not the best idea since we have to pass it around so much 
            var botId = result.self.id; 
            return await SlackRealTimeMessaging.Connect(new Uri(wsUrl), botId, new CancellationToken());
        }

        public async Task PostMessage(Response response)
        {
            var message = HttpUtility.UrlEncode(response.Message);
            var channel = HttpUtility.UrlEncode(response.Channel);
            var args = string.Format("&channel={0}&text={1}&parse=none&username=scbot", channel, message);
            if (response.Image != null)
            {
                args += "&icon_url=" + HttpUtility.UrlEncode(response.Image);
            }
            await GetApiResult("chat.postMessage", args);
        }

        private async Task<dynamic> GetApiResult(string apiEndpoint, string extraArgs = "")
        {
            using (var webClient = new WebClient())
            {
                var json = await webClient.DownloadStringTaskAsync(string.Format("https://slack.com/api/{0}?token={1}{2}", apiEndpoint, m_ApiKey, extraArgs));
                Console.WriteLine("Got: "+json);
                var result = Json.Decode(json);
                var ok = result.ok;
                if (!ok) throw new Exception("Error connecting to slack API: " + result.error);
                return result;
            }
        }
    }
}