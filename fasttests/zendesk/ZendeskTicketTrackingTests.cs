using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.processors;
using scbot.services;
using scbot.slack;
using scbot.services.compareengine;

namespace fasttests
{
    class ZendeskTicketTrackingTests
    {
        [Test]
        public void PostsUpdateWhenTrackedZendeskTicketChanges()
        {
            var zendeskApi = new Mock<IZendeskTicketApi>();
            var ticket1 = new ZendeskTicket("12345", "description", "open", new ZendeskTicket.Comment[3]);
            var ticket2 = new ZendeskTicket("12345", "description", "open", new ZendeskTicket.Comment[5]);
            var persistence = new InMemoryKeyValueStore();
            zendeskApi.Setup(x => x.FromId("12345")).ReturnsAsync(ticket1);

            var slackCommandParser = new SlackCommandParser("scbot", "U123");
            var zendeskTracker = new ZendeskTicketTracker(slackCommandParser, persistence, zendeskApi.Object);

            zendeskTracker.ProcessMessage(new Message("a-channel", "a-user", "scbot track ZD#12345"));

            CollectionAssert.IsEmpty(zendeskTracker.ProcessTimerTick().Responses);

            zendeskApi.Setup(x => x.FromId("12345")).ReturnsAsync(ticket2);

            var ping = zendeskTracker.ProcessTimerTick().Responses.Single();
            Assert.AreEqual("a-channel", ping.Channel);
            Assert.AreEqual("<https://redgatesupport.zendesk.com/agent/tickets/12345|ZD#12345> (description) updated: 2 comments added", ping.Message);

            // subsequent ticks should use updated values
            CollectionAssert.IsEmpty(zendeskTracker.ProcessTimerTick().Responses);
        }

        [Test]
        public void DoesNotUpdateIfZendeskApiReturnsNull()
        {
            var zendeskApi = new Mock<IZendeskTicketApi>();
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
        // TODO: we're currently updating tickets by id - we need to be untracking by id+channel and updating per-channel

        [Test]
        public void HasSpecificMessageForStatusChanged()
        {
            var persistence = new ListPersistenceApi<TrackedTicket>(new InMemoryKeyValueStore());
            var comparer = new ZendeskTicketCompareEngine(persistence);
            var responses = comparer.CompareTicketStates(new[]
            {
                new Update<ZendeskTicket>("a-channel", 
                    new ZendeskTicket("12345", "a-description", "open", new ZendeskTicket.Comment[3]),
                    new ZendeskTicket("12345", "a-description", "closed", new ZendeskTicket.Comment[3])),
            });
            Assert.AreEqual("<https://redgatesupport.zendesk.com/agent/tickets/12345|ZD#12345> (a-description) updated: `open` → `closed`", responses.Single().Message);
        }

        [Test]
        public void HasSpecificMessageForDescriptionChanged()
        {
            var persistence = new ListPersistenceApi<TrackedTicket>(new InMemoryKeyValueStore());
            var comparer = new ZendeskTicketCompareEngine(persistence);
            var responses = comparer.CompareTicketStates(new[]
            {
                new Update<ZendeskTicket>("a-channel",
                    new ZendeskTicket("12345", "a-description", "open", new ZendeskTicket.Comment[3]),
                    new ZendeskTicket("12345", "a-description updated", "open", new ZendeskTicket.Comment[3])),
            });
            Assert.AreEqual("<https://redgatesupport.zendesk.com/agent/tickets/12345|ZD#12345> (a-description updated) updated: description updated", responses.Single().Message);
        }

        [Test]
        public void GroupsTogetherMessagesForMultipleChanges()
        {
            var persistence = new ListPersistenceApi<TrackedTicket>(new InMemoryKeyValueStore());
            var comparer = new ZendeskTicketCompareEngine(persistence);
            var comment = new ZendeskTicket.Comment("a-comment", "some person", "an-avatar");
            var responses = comparer.CompareTicketStates(new[]
            {
                new Update<ZendeskTicket>("a-channel",
                    new ZendeskTicket("12345", "a-description", "open", new ZendeskTicket.Comment[0]),
                    new ZendeskTicket("12345", "a-description updated", "closed", new[] { comment })),
            });
            Assert.AreEqual("<https://redgatesupport.zendesk.com/agent/tickets/12345|ZD#12345> (a-description updated) updated: some person added a comment, `open` → `closed`, description updated", responses.Single().Message);
        }

        [Test]
        public void UsesAvatarAsImageIfSingleCommentPosted()
        {
            var persistence = new ListPersistenceApi<TrackedTicket>(new InMemoryKeyValueStore());
            var comparer = new ZendeskTicketCompareEngine(persistence);
            var comment = new ZendeskTicket.Comment("a-comment", "some person", "an-avatar");
            var responses = comparer.CompareTicketStates(new[]
            {
                new Update<ZendeskTicket>("a-channel",
                    new ZendeskTicket("12345", "a-description", "open", new ZendeskTicket.Comment[0]),
                    new ZendeskTicket("12345", "a-description updated", "closed", new[] { comment })),
            });
            Assert.AreEqual("an-avatar", responses.Single().Image);
        }
    }
}
