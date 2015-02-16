using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        [Test]
        public void CanCacheJiraApi()
        {
            var cached = new CachedJiraApi(new Time(), new JiraApi());
            TimeSpan uncachedTime, cachedTime;
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var bug = cached.FromId("SDC-1604").Result;
            uncachedTime = stopwatch.Elapsed;

            stopwatch.Reset();
            bug = cached.FromId("SDC-1604").Result;
            cachedTime = stopwatch.Elapsed;

            Assert.Greater(uncachedTime.TotalMilliseconds, 10);
            Assert.Less(cachedTime.TotalMilliseconds, 10);
        }
    }
}
