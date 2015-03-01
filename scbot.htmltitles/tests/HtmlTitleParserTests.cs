using Moq;
using NUnit.Framework;
using scbot.core.utils;
using scbot.htmltitles.services;
using System;
using System.Collections.Generic;

namespace scbot.htmltitles.tests
{
    public class HtmlTitleParserTests
    {
        [Test]
        public void CanFetchTitleForExampleDotCom()
        {
            var titleParser = new HtmlTitleParser(new WebClient());
            Assert.AreEqual("Example Domain", titleParser.GetHtmlTitle("http://example.com"));
        }

        [Test]
        public void DoesntThrowExceptionOnBadUrl()
        {
            var throwsException = new Mock<IWebClient>();
            throwsException.Setup(x => x.DownloadString(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).ThrowsAsync(new Exception());
            var titleParser = new HtmlTitleParser(throwsException.Object);
            Assert.AreEqual(null, titleParser.GetHtmlTitle("foo://bar.baz"));
        }

        [Test, Explicit] // tests #16
        public void CorrectlyFetchesUnicodeAndHtmlEscapedCharacters()
        {
            var titleParser = new HtmlTitleParser(new WebClient());
            var title = titleParser.GetHtmlTitle("https://twitter.com/SlackHQ/status/570695657561858048");
            StringAssert.DoesNotContain("&quot;", title);
            StringAssert.DoesNotContain("ðŸ“", title);
        }
    }
}
