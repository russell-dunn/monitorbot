using System.Threading;
using NUnit.Framework;
using scbot.core.utils;

namespace scbot.slack.tests
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
                var handler = new SlackMessageHandler(new LoggingBot(), rtm.BotId);
                while (true)
                {
                    var message = await rtm.Receive(new CancellationToken());
                    handler.Handle(message);
                }
            }
        }
    }
}
