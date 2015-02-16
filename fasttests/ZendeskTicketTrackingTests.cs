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
            Assert.AreEqual("Ticket <https://redgatesupport.zendesk.com/agent/tickets/12345|ZD#12345> was updated", ping.Message);
        }

        // TODO: check if already tracking
        // TODO: untrack
    }
}
