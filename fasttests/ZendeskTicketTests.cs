using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.processors;
using scbot.services;

namespace fasttests
{
    class ZendeskTicketTests
    {
        [TestCase("issue ZD#34182")]
        [TestCase("link <https://redgatesupport.zendesk.com/agent/tickets/34182>")]
        public void UsesZendeskApiToPrintBugReferenceDetails(string zendeskReference)
        {
            var api = new Mock<IZendeskApi>(MockBehavior.Strict);
            var ticket = new ZendeskTicket("34182", "SQL Packager 8 crash", "Closed", 45);
            api.Setup(x => x.FromId("34182")).ReturnsAsync(ticket);

            var processor = new ZendeskTicketProcessor(api.Object);
            var result = processor.ProcessMessage(new Message("a-channel", "a-user", string.Format("what is {0}", zendeskReference)));
            var response = result.Responses.Single();
            Assert.AreEqual("<https://redgatesupport.zendesk.com/agent/tickets/34182|ZD#34182> | SQL Packager 8 crash | Closed | 45 comments", response.Message);
        }

        [Test]
        public void DoesntMentionTheSameTicketTwice()
        {
            var api = new Mock<IZendeskApi>(MockBehavior.Strict);
            var ticket = new ZendeskTicket("34182", "SQL Packager 8 crash", "Closed", 45);
            api.Setup(x => x.FromId("34182")).ReturnsAsync(ticket);

            var processor = new ZendeskTicketProcessor(api.Object);
            var result = processor.ProcessMessage(new Message("a-channel", "a-user", string.Format("what is {0} and {0}", "ZD#34182")));
            var response = result.Responses.Single();
            Assert.AreEqual("<https://redgatesupport.zendesk.com/agent/tickets/34182|ZD#34182> | SQL Packager 8 crash | Closed | 45 comments", response.Message);
        }

        [Test]
        public void IgnoresNullOnError()
        {
            var api = new Mock<IZendeskApi>(MockBehavior.Strict);
            api.Setup(x => x.FromId("12345")).ReturnsAsync(null);

            var processor = new ZendeskTicketProcessor(api.Object);
            var result = processor.ProcessMessage(new Message("a-channel", "a-user", "what is ZD#12345"));
            CollectionAssert.IsEmpty(result.Responses);
        }
    }
}
