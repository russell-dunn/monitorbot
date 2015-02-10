using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot.services;

namespace slowtests
{
    class JiraIntegrationTests
    {
        [Test]
        public void CanFetchJiraIssueFromId()
        {
            var jira = new JiraApi();
            var bug = jira.FromId("SDC-1604").Result;
            Assert.AreEqual("Projects occasionally blow up when loaded against dbs with schema differences", bug.Title);
            Assert.AreEqual("Open", bug.Status);
            Assert.AreEqual(1, bug.CommentCount);
        }
    }
}
