﻿using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.services;

namespace fasttests
{
    public class HtmlTitleTests
    {
        // see https://api.slack.com/docs/formatting for how slack messages are formatted

        [Test]
        public void UsesHtmlTitleParserToGetTitlesForLinks()
        {
            var htmlTitleParser = new Mock<IHtmlTitleParser>();
            var url = "http://example.com";
            var title = "Example Domain";
            htmlTitleParser.Setup(x => x.GetHtmlTitle(url)).Returns(title);
            var htmlTitle = new HtmlTitleProcessor(htmlTitleParser.Object);
            var response = htmlTitle.ProcessMessage(new Message("a-channel", "some-user", string.Format("this is a link: <{0}>", url)));
            Assert.AreEqual(title, response.Responses.Single().Message);
        }

        [Test]
        public void UsesHtmlTitleParserToGetTitlesForLinksWithTitles()
        {
            var htmlTitleParser = new Mock<IHtmlTitleParser>();
            var url = "http://example.com";
            var title = "Example Domain";
            htmlTitleParser.Setup(x => x.GetHtmlTitle(url)).Returns(title);
            var htmlTitle = new HtmlTitleProcessor(htmlTitleParser.Object);
            var response = htmlTitle.ProcessMessage(new Message("a-channel", "some-user", string.Format("this is a link: <{0}|totally not a rickroll>", url)));
            Assert.AreEqual(title, response.Responses.Single().Message);
        }

        [Test]
        public void DoesntPrintAnythingForBadUrl()
        {
            var htmlTitleParser = new Mock<IHtmlTitleParser>();
            var url = "foo://bar.baz";
            string expectedTitle = null;
            htmlTitleParser.Setup(x => x.GetHtmlTitle(url)).Returns(expectedTitle);
            var htmlTitle = new HtmlTitleProcessor(htmlTitleParser.Object);
            var response = htmlTitle.ProcessMessage(new Message("a-channel", "some-user", string.Format("this is a link: <{0}>", url)));
            CollectionAssert.IsEmpty(response.Responses);
        }

    }
}
