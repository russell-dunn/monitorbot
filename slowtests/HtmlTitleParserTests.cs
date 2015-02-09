using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot.services;

namespace slowtests
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
