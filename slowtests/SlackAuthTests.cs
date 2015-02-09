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
            var rtm = await slackApi.StartRtm();
            var message = await rtm.Receive(new CancellationToken());
            Console.WriteLine(message.type);
        }
    }
}
