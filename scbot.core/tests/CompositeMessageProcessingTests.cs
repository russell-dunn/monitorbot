using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using scbot.core.bot;
using scbot.core.meta;

namespace scbot.core.tests
{
    class CompositeMessageProcessingTests
    {
        [Test]
        public void CompositeMessageProcessorGroupsTogetherResponses()
        {
            var subProcessor1 = new Mock<IMessageProcessor>();
            var subProcessor2 = new Mock<IMessageProcessor>();

            var response1 = new Response("a", "b");
            subProcessor1.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Returns(new MessageResult(new[] {response1,}));
            var response2 = new Response("c", "d", "image");
            subProcessor2.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Returns(new MessageResult(new[] {response2,}));

            var compositeMessageProcessor = new CompositeMessageProcessor(subProcessor1.Object, subProcessor2.Object);
            var result = compositeMessageProcessor.ProcessMessage(new Message("asdf", "a-user", "some-text"));
            CollectionAssert.AreEqual(new[] {response1, response2}, result.Responses);
        }

        [Test]
        public void ConcattingMessageProcessorConcatsTogetherResponsesToTheSameChannel()
        {
            var subProcessor = new Mock<IMessageProcessor>();

            var response1 = new Response("a", "1", "image1");
            var response2 = new Response("c", "2", "image2");
            var response3 = new Response("e", "2");
            subProcessor.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Returns(new MessageResult(new[] { response1, response2, response3}));

            var compositeMessageProcessor = new ConcattingMessageProcessor(subProcessor.Object);
            var result = compositeMessageProcessor.ProcessMessage(new Message("asdf", "a-user", "some-text"));
            Assert.AreEqual("a", result.Responses.ElementAt(0).Message);
            Assert.AreEqual("image1", result.Responses.ElementAt(0).Image);
            Assert.AreEqual("c\ne", result.Responses.ElementAt(1).Message);
            Assert.AreEqual("image2", result.Responses.ElementAt(1).Image);
        }

        [Test]
        public void ErrorCatchingMessageProcessorPassesThroughResults()
        {
            var underlying = new Mock<IMessageProcessor>();
            underlying.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Returns(new MessageResult(new[] {new Response("a", "b")}));

            var compositeMessageProcessor = new ErrorCatchingMessageProcessor(underlying.Object);
            var result = compositeMessageProcessor.ProcessMessage(new Message("asdf", "a-user", "some-text"));
            CollectionAssert.IsNotEmpty(result.Responses);
        }

        [Test]
        public void ErrorCatchingMessageProcessorHidesAllErrors()
        {
            var underlying = new Mock<IMessageProcessor>();
            underlying.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Throws(new Exception());

            var compositeMessageProcessor = new ErrorCatchingMessageProcessor(underlying.Object);
            var result = compositeMessageProcessor.ProcessMessage(new Message("asdf", "a-user", "some-text"));
            CollectionAssert.IsEmpty(result.Responses);
        }
    }
}
