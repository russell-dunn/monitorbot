using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot;
using scbot.slack;

namespace slowtests
{
    public class SlackAuthTests
    {
        [Test, Explicit]
        public async void CanGetRealTimeMessagingApi()
        {
            var slackApi = new SlackApi(Configuration.SlackApiKey);
            using (var rtm = await slackApi.StartRtm())
            {
                var json = await rtm.Receive(new CancellationToken());
                Assert.AreEqual("{\"type\":\"hello\"}", json);
            }
        }

        [Test, Explicit]
        public async void CanTestApi()
        {
            var slackApi = new SlackApi(Configuration.SlackApiKey);
            using (var rtm = await slackApi.StartRtm())
            {
                var handler = new SlackMessageHandler(new LoggingBot());
                while (true)
                {
                    var message = await rtm.Receive(new CancellationToken());
                    handler.Handle(message);
                }
            }
        }
    }
}
