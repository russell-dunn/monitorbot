using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.processors;
using scbot.services;

namespace fasttests
{
    public class JiraBugTests
    {
        [Test]
        public void UsesJiraApiToPrintBugReferenceDetails()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = new JiraBug("SDC-1604", "Projects occasionally blow up when loaded against dbs with schema differences", "Open", 1);
            jiraApi.Setup(x => x.FromId("SDC-1604")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "how about this bug: SDC-1604"));
            var response = result.Responses.Single();
            Assert.AreEqual("<https://jira.red-gate.com/browse/SDC-1604|SDC-1604> | Projects occasionally blow up when loaded against dbs with schema differences | Open | 1 comment", response.Message);
        }

        [Test]
        public void CanFetchMultipleBugsInMessages()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = new JiraBug("SDC-1604", "Projects occasionally blow up when loaded against dbs with schema differences", "Open", 1);
            jiraApi.Setup(x => x.FromId("SDC-1604")).ReturnsAsync(jiraBug);
            jiraApi.Setup(x => x.FromId("SC-1234")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "SC-1234 and SDC-1604 too"));
            Assert.AreEqual(2, result.Responses.Count());
        }

        [Test]
        public void DoesntMentionTheSameBugTwice()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = new JiraBug("SDC-1604", "Projects occasionally blow up when loaded against dbs with schema differences", "Open", 1);
            jiraApi.Setup(x => x.FromId("SC-1234")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "SC-1234 and SC-1234"));
            Assert.AreEqual(1, result.Responses.Count()); 
        }

        [Test]
        public void IgnoresNullsOnError()
        {
            var jiraApi = new Mock<IJiraApi>();
            jiraApi.Setup(x => x.FromId("NOT-3")).ReturnsAsync(null);

            var jiraBugProcessor = new JiraBugProcessor(jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "how about this bug: NOT-3"));
            CollectionAssert.IsEmpty(result.Responses);
        }
    }
}
