using Moq;
using NUnit.Framework;
using scbot.core.tests;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.github.services;

namespace scbot.labelprinting.tests
{
    class LabelPrintingTests
    {
        [Test]
        public void TestPrintLabel()
        {
            var webClient = new Mock<IWebClient>(MockBehavior.Strict);
            var processor = new LabelPrinting("fooCorp", 
                new GithubPRApi(webClient.Object, "githubToken"),
                new LabelPrinter("http://my_printer.com:9000", webClient.Object));

            webClient.Setup(x => x.DownloadString("https://api.github.com/repos/fooCorp/fooRepo/pulls/3",
                new[] { "Authorization: token githubToken" }))
                .ReturnsAsync(@"{ title: 'test pull request', body: 'test pull request body', user: { avatar_url: 'example image' } }");

            webClient.Setup(x => x.PostString("http://my_printer.com:9000", @"{""title"":""#3: test pull request"",""images"":[""https://assets-cdn.github.com/images/modules/logos_page/GitHub-Mark.png"",""example image"",""https://api.qrserver.com/v1/create-qr-code/?data=https://github.com/fooCorp/fooRepo/pull/3""]}", new[] { "content-type:application/json" })).ReturnsAsync("Printing ...");

            var result = processor.ProcessCommand("print label for fooRepo#3");

            Assert.AreEqual("Printing ...", result.Responses.Single().Message);
        }

        [Test]
        public void TestPrintLabelFromFullGithubReference()
        {
            var printer = new Mock<ILabelPrinter>(MockBehavior.Strict);
            var github = new Mock<IGithubPRApi>(MockBehavior.Strict);
            var processor = new LabelPrinting("fooCorp", github.Object, printer.Object);

            printer.Setup(x => x.PrintLabel("#3: test pull request", new List<string> { "https://assets-cdn.github.com/images/modules/logos_page/GitHub-Mark.png","example image","https://api.qrserver.com/v1/create-qr-code/?data=https://github.com/barCorp/barRepo/pull/3" } )).Returns("Printing ...");

            github.Setup(x => x.PullRequest("barCorp", "barRepo", 3)).ReturnsAsync(new { title = "test pull request", user = new { avatar_url = "example image" } });

            var result = processor.ProcessCommand("print label for barCorp/barRepo#3");

            Assert.AreEqual("Printing ...", result.Responses.Single().Message);
        }
    }
}
