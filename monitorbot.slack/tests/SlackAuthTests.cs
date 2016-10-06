using System.Threading;
using monitorbot.core.utils;
using NUnit.Framework;

namespace monitorbot.slack.tests
{
    public class SlackAuthTests
    {
        private Configuration m_Configuration = Configuration.Load();

        [Test, Explicit]
        public async void CanGetRealTimeMessagingApi()
        {
            var slackApi = new SlackApi(m_Configuration.Get("slack-api-key"));
            using (var rtm = await slackApi.StartRtm())
            {
                var json = await rtm.Receive(new CancellationToken());
                Assert.AreEqual("{\"type\":\"hello\"}", json);
            }
        }

        [Test, Explicit]
        public async void CanTestApi()
        {
            var slackApi = new SlackApi(m_Configuration.Get("slack-api-key"));
            using (var rtm = await slackApi.StartRtm())
            {
                var handler = new SlackMessageHandler(new LoggingBot(), rtm.InstanceInfo.BotId);
                while (true)
                {
                    var message = await rtm.Receive(new CancellationToken());
                    handler.Handle(message);
                }
            }
        }
    }
}
