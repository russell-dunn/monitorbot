using Moq;
using NUnit.Framework;
using monitorbot.core.meta;
using monitorbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.bot;

namespace monitorbot.core.tests
{
    [TestFixture]
    public class FeatureMessageProcessorTests
    {
        [Test]
        public void DelegatesMessageProcessingToUnderlyingProcessor()
        {
            var underlying = new Mock<IMessageProcessor>();
            underlying.Setup(x => x.ProcessMessage(It.IsAny<Message>())).Returns(new Response("", ""));
            var feature = new BasicFeature("test", "test feature please ignore", "this is a test feature", underlying.Object);

            var commandParser = new Mock<ICommandParser>();
            var processor = new FeatureMessageProcessor(commandParser.Object, feature);

            var message = new Message("a-channel", "a-user", "some random message text");
            var result = processor.ProcessMessage(message).Responses.Single();

            underlying.Verify(x => x.ProcessMessage(message));
        }

        [Test]
        public void ReturnsHelpTextForHelpCommand()
        {
            var underlying = new Mock<IMessageProcessor>();
            var feature1 = new BasicFeature("test", "test feature please ignore", "this is a test feature", underlying.Object);
            var feature2 = new BasicFeature("test2", "test feature please ignore", "this is another test feature", underlying.Object);
            var commandParser = new Mock<ICommandParser>();
            commandParser.SetupTryGetCommand("help");

            var processor = new FeatureMessageProcessor(commandParser.Object, feature1, feature2);

            var message = new Message("a-channel", "a-user", "help");
            var result = processor.ProcessMessage(message);

            Assert.AreEqual("*test*: test feature please ignore\n*test2*: test feature please ignore\n\nuse `help <feature>` for help on a specific feature", 
                result.Responses.Single().Message);
            underlying.Verify(x => x.ProcessMessage(message), Times.Never);
        }

        [Test]
        public void ReturnsSpecificHelpTextForSpecificHelp()
        {
            var underlying = new Mock<IMessageProcessor>();
            var feature1 = new BasicFeature("test", "test feature please ignore", "this is a test feature", underlying.Object);
            var feature2 = new BasicFeature("test2", "test feature please ignore", "this is another test feature", underlying.Object);
            var commandParser = new Mock<ICommandParser>();
            commandParser.SetupTryGetCommand("help test2");

            var processor = new FeatureMessageProcessor(commandParser.Object, feature1, feature2);

            var message = new Message("a-channel", "a-user", "help test2");
            var result = processor.ProcessMessage(message);

            Assert.AreEqual("*test2*: test feature please ignore\nthis is another test feature", result.Responses.Single().Message);
            underlying.Verify(x => x.ProcessMessage(message), Times.Never);
        }

        [Test]
        public void ReturnsErrorIfSpecificHelpNotFound()
        {
            var underlying = new Mock<IMessageProcessor>();
            var feature1 = new BasicFeature("test", "test feature please ignore", "this is a test feature", underlying.Object);
            var feature2 = new BasicFeature("test2", "test feature please ignore", "this is another test feature", underlying.Object);
            var commandParser = new Mock<ICommandParser>();
            commandParser.SetupTryGetCommand("help test3");

            var processor = new FeatureMessageProcessor(commandParser.Object, feature1, feature2);

            var message = new Message("a-channel", "a-user", "help test3");
            var result = processor.ProcessMessage(message);

            Assert.AreEqual("Sorry, I don't know about 'test3'.", result.Responses.Single().Message);
            underlying.Verify(x => x.ProcessMessage(message), Times.Never);
        }
    }
}
