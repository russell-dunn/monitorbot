using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.bot;
using monitorbot.core.meta;
using monitorbot.core.utils;
using Moq;
using NUnit.Framework;

namespace monitorbot.core.tests
{
    class ErrorReportingMessageProcessorTests
    {
        [Test]
        public void ErrorCatchingMessageProcessorPassesThroughResults()
        {
            var underlying = new Mock<IMessageProcessor>();
            underlying.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Returns(new Response("a", "b"));

            var pastebin = new Mock<IPasteBin>();

            var processor = new ErrorReportingMessageProcessor(underlying.Object, pastebin.Object);
            var result = processor.ProcessMessage(new Message("asdf", "a-user", "some-text"));
            CollectionAssert.IsNotEmpty(result.Responses);
        }

        [Test]
        public void PrintsMessageWhenExceptionHappens()
        {
            var underlying = new Mock<IMessageProcessor>();
            underlying.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Throws(new ArgumentNullException("test exception please ignore"));

            var pastebin = new Mock<IPasteBin>();
            pastebin.Setup(x => x.UploadPaste(It.IsAny<string>())).Returns("http://pastebin/link/to/paste");

            var processor = new ErrorReportingMessageProcessor(underlying.Object, pastebin.Object);
            var result = processor.ProcessMessage(new Message("asdf", "a-user", "some-text"));
            Assert.AreEqual("DANGER WILL ROBINSON: A <http://pastebin/link/to/paste|ArgumentNullException> was encountered while processing this request.", result.Responses.Single().Message);
        }
    }
}
