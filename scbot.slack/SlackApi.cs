using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using scbot.core.bot;
using System.Diagnostics;
using System.Collections.Generic;

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
            PrintRtmData(result);
            var wsUrl = result.url;
            // TODO: getting the bot id here seems to be the most convenient but it's probably not the best idea since we have to pass it around so much 
            var botId = result.self.id;
            var users = GetUserList(result.users);
            var instanceInfo = new SlackInstanceInfo(botId, users);
            return await SlackRealTimeMessaging.Connect(new Uri(wsUrl), instanceInfo, new CancellationToken());
        }

        private IEnumerable<SlackUser> GetUserList(dynamic users)
        {
            var result = new List<SlackUser>();
            foreach (var user in users)
            {
                var displayName = String.IsNullOrWhiteSpace(user.real_name) ? user.name : user.real_name;
                result.Add(new SlackUser(displayName, user.name, user.id));
            }
            return result;
        }

        private void PrintRtmData(dynamic result)
        {
            try
            {
                Trace.WriteLine((string)string.Format("I am {0} ({1})", result.self.name, result.self.id));
                Trace.WriteLine((string)string.Format("I am connecting to {0} ({1})", result.team.name, result.team.id));
                Trace.WriteLine("Users:");
                PrintThingList(result.users);
                Trace.WriteLine("Channels:");
                PrintThingList(result.channels);
                Trace.WriteLine("IMs:");
                PrintThingList(result.ims);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error reading rtm.start result: " + e);
            }
        }

        private void PrintThingList(dynamic things)
        {
            foreach (var thing in things)
            {
                Trace.WriteLine((string)string.Format("  {0} ({1})", thing.name ?? thing.user, thing.id));
            }
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
                var result = Json.Decode(json);
                var ok = result.ok;
                if (!ok) throw new Exception("Error connecting to slack API: " + result.error);
                return result;
            }
        }
    }
}