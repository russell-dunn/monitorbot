using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot.email.services;
using scbot.email.tests;
using scbot.labelprinting;

namespace scbot.rg.tests
{
    public class CompareTeamEmailsTests
    {
        [Test]
        public void IgnoresEmailsToOtherInboxes()
        {
            var email = new Email("a-subject", "html body", "slack body", "asdf", DateTime.Now,
                new List<string> {"a-personal-address@red-gate.com"}, true);
            var processor = new CompareTeamEmails("email-channel", new Mock<ILabelPrinter>().Object);

            processor.Accept(email);
            var result = processor.ProcessTimerTick();
            CollectionAssert.IsEmpty(result.Responses);
        }

        [Test]
        public void IgnoresFollowupEmails()
        {
            var isFirstEmailInConversation = false;
            var email = new Email("a-subject", "html body", "slack body", "asdf", DateTime.Now,
                new List<string> { "sqlcomparesupport@red-gate.com" }, isFirstEmailInConversation);
            var processor = new CompareTeamEmails("email-channel", new Mock<ILabelPrinter>().Object);

            processor.Accept(email);
            var result = processor.ProcessTimerTick();
            CollectionAssert.IsEmpty(result.Responses);
        }

        [Test]
        public void PrintsMessageForEmailToSupportInbox()
        {
            var email = new Email("a-subject", "html body", "slack body", "asdf", DateTime.Now,
                new List<string> { "sqlcomparesupport@red-gate.com" }, true);
            var processor = new CompareTeamEmails("email-channel", new Mock<ILabelPrinter>().Object);

            processor.Accept(email);
            var result = processor.ProcessTimerTick().Responses.Single();
            Assert.AreEqual("email-channel", result.Channel);
            Assert.AreEqual("New email sent to sqlcomparesupport@red-gate.com\n**a-subject**\nslack body", 
                result.Message);
        }

        [Test]
        public void PrintsSpecialMessageForZendeskEscalationEmails()
        {
            var escalationMessageBody = MessageParsingTests.EscalationMessageText;
            var email = new Email("a-subject", escalationMessageBody, EmailParser.GetSlackFormattedSummary(escalationMessageBody), "asdf", DateTime.Now,
                 new List<string> { "sqlcomparesupport@red-gate.com" }, true);
            var processor = new CompareTeamEmails("email-channel", new Mock<ILabelPrinter>().Object);

            processor.Accept(email);
            var result = processor.ProcessTimerTick().Responses.Single();
            Assert.AreEqual("email-channel", result.Channel);
            Assert.AreEqual("New support escalation for ZD#41572\n**a-subject**\n*Rob Clenshaw* (Support)\nMay 28, 14:57 \nHe\'s not happy :(\nIs there anything we can do?\n*Ticket #* 41572 \n*Status* On-hold \n*Requester* A Customer  \n*CCs* - \n*Group* SQL \n*Assignee* Rob Clenshaw \n*Priority* Normal \n*Type* Incident \n*Channel* By mail \n&nbsp;\n<http://www.zendesk.com|Zendesk>. Message-Id:NJGVB39A_55671ee12ddf9_a3103fe9d48cd3382809a3_sprutTicket-Id:41572Account-Subdomain:redgatesupport",
                result.Message); 
        }
    }
}
