using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using scbot.core.utils;
using scbot.zendesk.services;

namespace scbot.zendesk.tests
{
    class ZendeskIntegrationTests
    {
        private Configuration m_Configuration = Configuration.Load();

        [Test, Explicit]
        public void CanCreateZendeskApiAndFetchIssue()
        {
            var api = new ZendeskTicketApi(ZendeskApi.Create(m_Configuration.Get("redgate-id")));
            var ticket = api.FromId("34182").Result;
            Assert.AreEqual("SQL Packager 8 crash", ticket.Description);
            var comment = ticket.Comments.ElementAt(1);
            Assert.NotNull(comment.Author);
            Assert.NotNull(comment.Avatar);
            Trace.WriteLine(comment.Author);
            Trace.WriteLine(comment.Avatar);
        }

        [Test, Explicit]
        public void CanCacheZendeskApi()
        {
            var cached = new CachedZendeskApi(new Time(), ZendeskApi.Create(m_Configuration.Get("redgate-id")));
            TimeSpan uncachedTime, cachedTime;
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var bug = cached.Ticket("34182").Result;
            uncachedTime = stopwatch.Elapsed;

            stopwatch.Reset();
            bug = cached.Ticket("34182").Result;
            cachedTime = stopwatch.Elapsed;

            Assert.Greater(uncachedTime.TotalMilliseconds, 10);
            Assert.Less(cachedTime.TotalMilliseconds, 10);
        }

        [Test, Explicit]
        public void ReturnsNullOnError()
        {
            var api = new ErrorCatchingZendeskTicketApi(new ZendeskTicketApi(ZendeskApi.Create(m_Configuration.Get("redgate-id"))));
            Assert.AreEqual(default(ZendeskTicket), api.FromId("not a zd id").Result); 
        }
    }
}
