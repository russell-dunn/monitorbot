using Moq;
using NUnit.Framework;
using scbot.core.tests;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.labelprinting.tests
{
    class LabelPrintingTests
    {
        [Test]
        public void TestPrintLabel()
        {
            var webClient = new Mock<IWebClient>(MockBehavior.Strict);
            IWebClient webClient1 = webClient.Object;
            var processor = new LabelPrinting(webClient1, "fooCorp", "githubToken",  new LabelPrinter("http://my_printer.com:9000", webClient1));

            webClient.Setup(x => x.DownloadString("https://api.github.com/repos/fooCorp/fooRepo/pulls/3",
                new[] { "Authorization: token githubToken" }))
                .ReturnsAsync(@"{ title: 'test pull request', body: 'test pull request body', user: { avatar_url: 'example image' } }");

            webClient.Setup(x => x.PostString("http://my_printer.com:9000", @"{""title"":""#3: test pull request"",""images"":[""https://assets-cdn.github.com/images/modules/logos_page/GitHub-Mark.png"",""example image"",""https://api.qrserver.com/v1/create-qr-code/?data=https://github.com/fooCorp/fooRepo/pull/3""]}", new[] { "content-type:application/json" })).ReturnsAsync("Printing ...");

            var result = processor.ProcessCommand("print label for fooRepo#3");

            Assert.AreEqual("Printing ...", result.Responses.Single().Message);
        }
    }
}
