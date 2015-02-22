using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using scbot;
using scbot.services;

namespace slowtests
{
    class ZendeskIntegrationTests
    {
        [Test, Explicit]
        public void CanCreateZendeskApiAndFetchIssue()
        {
            var api = new ZendeskTicketApi(ZendeskApi.Create(Configuration.RedgateId));
            var ticket = api.FromId("34182").Result;
            Assert.AreEqual("SQL Packager 8 crash", ticket.Description);
        }

        [Test, Explicit]
        public void CanCacheZendeskApi()
        {
            var cached = new CachedZendeskApi(new Time(), new ZendeskTicketApi(ZendeskApi.Create(Configuration.RedgateId)));
            TimeSpan uncachedTime, cachedTime;
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var bug = cached.FromId("34182").Result;
            uncachedTime = stopwatch.Elapsed;

            stopwatch.Reset();
            bug = cached.FromId("34182").Result;
            cachedTime = stopwatch.Elapsed;

            Assert.Greater(uncachedTime.TotalMilliseconds, 10);
            Assert.Less(cachedTime.TotalMilliseconds, 10);
        }

        [Test, Explicit]
        public void ReturnsNullOnError()
        {
            var api = new ErrorCatchingZendeskTicketApi(new ZendeskTicketApi(ZendeskApi.Create(Configuration.RedgateId)));
            Assert.AreEqual(default(ZendeskTicket), api.FromId("not a zd id").Result); 
        }
    }
}
