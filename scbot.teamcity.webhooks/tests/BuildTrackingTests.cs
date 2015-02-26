using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot.bot;
using scbot.core.tests;
using scbot.services.compareengine;
using scbot.services.persistence;
using scbot.services.teamcity;

namespace scbot.teamcity.webhooks.tests
{
    class BuildTrackingTests
    {
        [Test]
        public void StartsTrackingBuild()
        {
            var persistence = new Mock<IListPersistenceApi<Tracked<Build>>>();
            var commandParser = CommandlineParser.For("track build 12345");
            using (var processor = TeamcityWebhooksMessageProcessor.Start(persistence.Object, commandParser, "http://localhost:7357"))
            {
                var response = processor.ProcessMessage(new Message("a-channel", "a-user", "scbot track build 12345"));
                Assert.AreEqual("Now tracking build#12345", response.Responses.Single().Message);
                persistence.Verify(x => x.AddToList("tcwh-tracked-builds", new Tracked<Build>(new Build("12345"), "a-channel")));
            }
        }
    }
}
