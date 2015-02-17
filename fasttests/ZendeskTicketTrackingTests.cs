using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.processors;
using scbot.services;
using scbot.slack;

namespace fasttests
{
    class ZendeskTicketTrackingTests
    {
        [Test]
        public void PostsUpdateWhenTrackedZendeskTicketChanges()
        {
            var zendeskApi = new Mock<IZendeskApi>();
            var ticket1 = new ZendeskTicket("12345", "description", "open", 3);
            var ticket2 = new ZendeskTicket("12345", "description", "open", 5);
            var persistence = new InMemoryKeyValueStore();
            zendeskApi.Setup(x => x.FromId("12345")).ReturnsAsync(ticket1);

            var slackCommandParser = new SlackCommandParser("scbot", "U123");
            var zendeskTracker = new ZendeskTicketTracker(slackCommandParser, persistence, zendeskApi.Object);

            zendeskTracker.ProcessMessage(new Message("a-channel", "a-user", "scbot track ZD#12345"));

            CollectionAssert.IsEmpty(zendeskTracker.ProcessTimerTick().Responses);

            zendeskApi.Setup(x => x.FromId("12345")).ReturnsAsync(ticket2);

            var ping = zendeskTracker.ProcessTimerTick().Responses.Single();
            Assert.AreEqual("a-channel", ping.Channel);
            Assert.AreEqual("2 comment(s) were added to <https://redgatesupport.zendesk.com/agent/tickets/12345|ZD#12345>", ping.Message);

            // subsequent ticks should use updated values
            CollectionAssert.IsEmpty(zendeskTracker.ProcessTimerTick().Responses);
        }

        [Test]
        public void DoesNotUpdateIfZendeskApiReturnsNull()
        {
            var zendeskApi = new Mock<IZendeskApi>();
            zendeskApi.Setup(x => x.FromId(It.IsAny<string>())).ReturnsAsync(default(ZendeskTicket));
            var initialJson = @"[{""Ticket"":{""Id"":""12345"",""Description"":""the description"",""Status"":""hold"",""CommentCount"":5},""Channel"":""a-channel""}]";
            var persistence = new Mock<IKeyValueStore>();
            persistence.Setup(x => x.Get("tracked-zd-tickets")).Returns(initialJson);

            var slackCommandParser = new SlackCommandParser("scbot", "U123");
            var zendeskTracker = new ZendeskTicketTracker(slackCommandParser, persistence.Object, zendeskApi.Object);

            zendeskTracker.ProcessTimerTick();

            persistence.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void CanUntrackTickets()
        {
            var initialJson = @"[{""Ticket"":{""Id"":""12345"",""Description"":""the description"",""Status"":""hold"",""CommentCount"":5},""Channel"":""a-channel""}]";

            var persistence = new Mock<IKeyValueStore>();
            persistence.Setup(x => x.Get("tracked-zd-tickets")).Returns(initialJson);
            
            var slackCommandParser = new SlackCommandParser("scbot", "U123");
            var zendeskTracker = new ZendeskTicketTracker(slackCommandParser, persistence.Object, null);

            var response = zendeskTracker.ProcessMessage(new Message("a-channel", "a-user", "scbot untrack ZD#12345")).Responses.Single();

            Assert.AreEqual("No longer tracking ZD#12345.", response.Message);
            persistence.VerifyAll();
            persistence.Verify(x => x.Set("tracked-zd-tickets", "[]"));
        }

        // TODO: check if already tracking
        // TODO: more specific diffs (particularly status changed or comments added)
    }
}
