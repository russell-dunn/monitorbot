using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.services;

namespace fasttests
{
    public class JiraBugTests
    {
        [Test]
        public void UsesJiraApiToPrintBugReferenceDetails()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = new JiraBug("Projects occasionally blow up when loaded against dbs with schema differences", "Open", 1);
            jiraApi.Setup(x => x.FromId("SDC-1604")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "how about this bug: SDC-1604"));
            var response = result.Responses.Single();
            Assert.AreEqual("<https://jira.red-gate.com/browse/SDC-1604|SDC-1604> | Projects occasionally blow up when loaded against dbs with schema differences | 1 comment", response.Message);
        }

        [Test]
        public void DoesNotConsiderBugReferencesInLinks()
        {
            // TODO - want to change this behaviour so that jira bug links use this instead of HtmlTitleParser
        }
    }
}
