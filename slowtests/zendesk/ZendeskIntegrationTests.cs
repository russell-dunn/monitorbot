using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using scbot.services;
using scbot.services.zendesk;
using scbot.utils;

namespace slowtests.zendesk
{
    class ZendeskIntegrationTests
    {
        [Test, Explicit]
        public void CanCreateZendeskApiAndFetchIssue()
        {
            var api = new ZendeskTicketApi(ZendeskApi.Create(Configuration.RedgateId));
            var ticket = api.FromId("34182").Result;
            Assert.AreEqual("SQL Packager 8 crash", ticket.Description);
            var comment = ticket.Comments.ElementAt(1);
            Assert.NotNull(comment.Author);
            Assert.NotNull(comment.Avatar);
            Console.WriteLine(comment.Author);
            Console.WriteLine(comment.Avatar);
        }

        [Test, Explicit]
        public void CanCacheZendeskApi()
        {
            var cached = new CachedZendeskApi(new Time(), ZendeskApi.Create(Configuration.RedgateId));
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
            var api = new ErrorCatchingZendeskTicketApi(new ZendeskTicketApi(ZendeskApi.Create(Configuration.RedgateId)));
            Assert.AreEqual(default(ZendeskTicket), api.FromId("not a zd id").Result); 
        }
    }
}
