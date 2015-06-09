using Moq;
using NUnit.Framework;
using scbot.core.utils;
using scbot.zendesk.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace scbot.zendesk.tests
{
    [TestFixture]
    public class ReconnectingApiTests
    {
        [Test]
        public void BacksOffAfterThrowingException()
        {
            var underlying = new Mock<IZendeskApi>(MockBehavior.Strict);
            var time = new Mock<ITime>();
            time.Setup(x => x.Now).Returns(DateTime.Now);

            underlying.Setup(x => x.Ticket("12345")).Throws<WebException>();

            var api = ReconnectingZendeskApi.CreateAsync(() => Task.FromResult(underlying.Object), time.Object).Result;

            Assert.Catch(() => { var foo = api.Ticket("12345").Result; });
            Assert.Null(api.Ticket("12345").Result);

            time.Setup(x => x.Now).Returns(DateTime.Now + 3.Minutes());

            Assert.Catch(() => { var foo = api.Ticket("12345").Result; });
        }
    }
}
