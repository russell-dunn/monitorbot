using Moq;
using NUnit.Framework;
using scbot.core.bot;
using scbot.core.tests;
using scbot.review.reviewer;
using scbot.review.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.tests
{
    class ReviewTests
    {
        [Test]
        public void ReturnsCommentsFromReviewApi()
        {
            var reviewApi = new Mock<IReviewApi>();
            reviewApi.Setup(x => x.ReviewForPullRequest("fooCorp", "fooMatic", 123)).ReturnsAsync(
                new[] { new DiffComment("sucks", "this sucks", "f", 1), new DiffComment("rocks", "this rocks", "g", 2) });
            var commandParser = CommandlineParser.For("review #123");
            var processor = new GithubReviewMessageProcessor(commandParser, reviewApi.Object, "fooCorp", "fooMatic");

            var response = processor.ProcessMessage(new Message("a-channel", "a-user", "msg"));

            reviewApi.VerifyAll();
            Assert.AreEqual(2, response.Responses.Count());
        }

        [Test]
        public void CanParseGithubReferences()
        {
            var reviewApi = new Mock<IReviewApi>();
            reviewApi.Setup(x => x.ReviewForPullRequest("initech", "iniRepo", 123)).ReturnsAsync(
                new[] { new DiffComment("sucks", "this sucks", "f", 1), new DiffComment("rocks", "this rocks", "g", 2) });

            var commandParser = CommandlineParser.For("review initech/iniRepo#123");

            var processor = new GithubReviewMessageProcessor(commandParser, reviewApi.Object, "fooCorp", "fooMatic");

            processor.ProcessMessage(new Message("a-channel", "a-user", "msg"));

            reviewApi.VerifyAll();
        }
    }
}
