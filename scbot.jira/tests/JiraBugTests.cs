using System.Linq;
using Moq;
using NUnit.Framework;
using scbot.core.bot;
using scbot.jira.services;
using scbot.core.tests;

namespace scbot.jira.tests
{
    public class JiraBugTests
    {
        private static JiraBug CreateJiraBug()
        {
            return new JiraBug("SDC-1604", "Bug", "Projects occasionally blow up when loaded against dbs with schema differences", "desc", "Open", 1);
        }

        [Test]
        public void UsesJiraApiToPrintBugReferenceDetails()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = CreateJiraBug();
            jiraApi.Setup(x => x.FromId("SDC-1604")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(CommandParser.For(""), jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "how about this bug: SDC-1604"));
            var response = result.Responses.Single();
            Assert.AreEqual("<https://jira.red-gate.com/browse/SDC-1604|SDC-1604> | Bug | Projects occasionally blow up when loaded against dbs with schema differences | Open | 1 comment", response.Message);
        }

        [Test]
        public void CanFetchMultipleBugsInMessages()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = CreateJiraBug();
            jiraApi.Setup(x => x.FromId("SDC-1604")).ReturnsAsync(jiraBug);
            jiraApi.Setup(x => x.FromId("SC-1234")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(CommandParser.For(""), jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "SC-1234 and SDC-1604 too"));
            Assert.AreEqual(2, result.Responses.Count());
        }

        [Test]
        public void DoesntMentionTheSameBugTwice()
        {
            var jiraApi = new Mock<IJiraApi>();
            var jiraBug = CreateJiraBug();
            jiraApi.Setup(x => x.FromId("SC-1234")).ReturnsAsync(jiraBug);

            var jiraBugProcessor = new JiraBugProcessor(CommandParser.For(""), jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "SC-1234 and SC-1234"));
            Assert.AreEqual(1, result.Responses.Count()); 
        }

        [Test]
        public void IgnoresNullsOnError()
        {
            var jiraApi = new Mock<IJiraApi>();
            jiraApi.Setup(x => x.FromId("NOT-3")).ReturnsAsync(null);

            var jiraBugProcessor = new JiraBugProcessor(CommandParser.For(""), jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "how about this bug: NOT-3"));
            CollectionAssert.IsEmpty(result.Responses);
        }

        [Test]
        public void SuggestsLabelsForBugs()
        {
            var jiraApi = new Mock<IJiraApi>();
            var bug = new JiraBug("SC-1234", "AutoBug", "OutOfMemoryException", "{\n\"MethodTypeName\": \"[RedGate.Shared.SQL]RedG", "Open", 12);
            jiraApi.Setup(x => x.FromId("SC-1234")).ReturnsAsync(bug);

            var commandParser = CommandParser.For("suggest labels for SC-1234");
            var jiraBugProcessor = new JiraLabelSuggester(commandParser, jiraApi.Object);
            var result = jiraBugProcessor.ProcessMessage(new Message("a-channel", "a-user", "msg"));
            Assert.AreEqual("bugtype:oom repo:sharedsql", result.Responses.Single().Message);
        }

        // TODO: should ignore bug in urls like https://github.com/red-gate/SQLCompareEngine/compare/bug/SC-7710#diff-7a4ccf95c069231db2c74c5866f7c6b9R14 ? 
    }
}
