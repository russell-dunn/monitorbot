using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var api = ZendeskApi.Create(Configuration.RedgateId);
            var ticket = api.FromId("34182").Result;
            Assert.AreEqual("SQL Packager 8 crash", ticket.Description);
        }
    }
}
