using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot.core.bot;
using scbot.core.tests;

namespace scbot.polls.tests
{
    [TestFixture]
    public class PollTests
    {
        [Test]
        public void ComplainsIfNoTestIsRunning()
        {
            var polls = new Polls(CommandParser.For("vote 1"));
            var result = polls.ProcessMessage();
            Assert.AreEqual("The poll is not currently running. Use `scbot: poll start` to start a poll.", result.Responses.Single().Message);
        }

        [Test]
        public void CanCreateAPoll()
        {
            var polls = new Polls(CommandParser.For("start poll"));
            var result = polls.ProcessMessage();
            Assert.AreEqual("Polling started. Use `scbot: poll add <some choice>` to add choices " + 
                "and `scbot: vote 1` to vote for a particular option, then `scbot: poll finish` to show the results.", result.Responses.Single().Message);
        }

        [Test]
        public void ComplainsIfPollCreatedWhilePollIsRunning()
        {
            var polls = new Polls(CommandParser.For("start poll"));
            polls.ProcessMessage(new Message("channel", "user", "message"));
            var result = polls.ProcessMessage(new Message("a-channel", "user", "text"));
            Assert.AreEqual("A poll is already running in a-channel", result.Responses.Single().Message);
        }

        [Test]
        public void CanAddOptionToPoll()
        {
            var commandParser = new Mock<ICommandParser>();
            var polls = new Polls(commandParser.Object);
            commandParser.SetupTryGetCommand("poll start");
            polls.ProcessMessage();
            commandParser.SetupTryGetCommand("poll add some choice");
            var result = polls.ProcessMessage();
            Assert.AreEqual("Choice added. Use `scbot: vote 1` to vote for it.", result.Responses.Single().Message); 
        }

        [Test]
        public void CanVoteForOption()
        {
            var commandParser = new Mock<ICommandParser>();
            var polls = new Polls(commandParser.Object);
            commandParser.SetupTryGetCommand("poll start");
            polls.ProcessMessage();
            commandParser.SetupTryGetCommand("poll add some choice");
            polls.ProcessMessage();
            commandParser.SetupTryGetCommand("vote 1");
            var result = polls.ProcessMessage();
            Assert.AreEqual("Vote added for choice 1 (some choice). You can change your vote by voting for something else.", result.Responses.Single().Message);
        }

        [Test]
        public void PrintsResultsWhenPollIsClosed()
        {
            var commandParser = new Mock<ICommandParser>();
            var polls = new Polls(commandParser.Object);
            commandParser.SetupTryGetCommand("poll start");
            polls.ProcessMessage();
            commandParser.SetupTryGetCommand("poll add some choice");
            polls.ProcessMessage();
            commandParser.SetupTryGetCommand("poll add some other choice");
            polls.ProcessMessage();
            commandParser.SetupTryGetCommand("vote 1");
            polls.ProcessMessage(new Message("channel", "user-1", "message"));
            polls.ProcessMessage(new Message("channel", "user-2", "message"));
            commandParser.SetupTryGetCommand("vote 2");
            polls.ProcessMessage(new Message("channel", "user-3", "message"));

            commandParser.SetupTryGetCommand("poll finish");
            var result = polls.ProcessMessage();
            Assert.AreEqual("Polls closed! The winner is choice 1 (some choice).\n`##` some choice\n`#` some other choice", result.Responses.Single().Message);
        }
    }
}
