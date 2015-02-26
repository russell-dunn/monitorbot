using NUnit.Framework;
using scbot.htmltitles.services;

namespace scbot.htmltitles.tests
{
    public class HtmlTitleParserTests
    {
        [Test]
        public void CanFetchTitleForExampleDotCom()
        {
            var titleParser = new HtmlTitleParser();
            Assert.AreEqual("Example Domain", titleParser.GetHtmlTitle("http://example.com"));
        }

        [Test]
        public void DoesntThrowExceptionOnBadUrl()
        {
            var titleParser = new HtmlTitleParser();
            Assert.AreEqual(null, titleParser.GetHtmlTitle("foo://bar.baz"));
        }
    }
}
