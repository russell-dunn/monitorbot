using System;
using System.Diagnostics;
using NUnit.Framework;
using scbot.services;
using scbot.services.jira;

namespace slowtests.jira
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

        [Test]
        public void ReturnsNullOnError()
        {
            var jira = new JiraApi();
            Assert.Null(jira.FromId("UTF-8").Result);
        }
    }
}
