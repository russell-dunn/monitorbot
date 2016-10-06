using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.utils;
using Moq;
using NUnit.Framework;
using monitorbot.core.bot;
using monitorbot.core.tests;

namespace monitorbot.polls.tests
{
    [TestFixture]
    public class PollTests
    {
        [Test]
        public void ComplainsIfNoTestIsRunning()
        {
            var polls = new Polls();
            var result = polls.ProcessCommand("vote 1");
            Assert.AreEqual("The poll is not currently running. Use `poll start` to start a poll.", result.Responses.Single().Message);
        }

        [Test]
        public void CanCreateAPoll()
        {
            var polls = new Polls();
            var result = polls.ProcessCommand("start poll");
            Assert.AreEqual("Polling started. Use `poll add <some choice>` to add choices " + 
                "and `vote 1` to vote for a particular option, then `poll finish` to show the results.", result.Responses.Single().Message);
        }

        [Test]
        public void ComplainsIfPollCreatedWhilePollIsRunning()
        {
            var polls = new Polls();
            polls.ProcessCommand(new Command("poll-channel", "user", "start poll"));
            var result = polls.ProcessCommand(new Command("other-channel", "user", "start poll"));
            Assert.AreEqual("A poll is already running in poll-channel", result.Responses.Single().Message);
        }

        [Test]
        public void CanAddOptionToPoll()
        {
            var polls = new Polls();
            polls.ProcessCommand("poll start");
            var result = polls.ProcessCommand("poll add some choice");
            Assert.AreEqual("Choice added. Use `vote 1` to vote for it.", result.Responses.Single().Message); 
        }

        [Test]
        public void CanVoteForOption()
        {
            var polls = new Polls();
            polls.ProcessCommand("poll start");
            polls.ProcessCommand("poll add some choice");
            var result = polls.ProcessCommand("vote 1");
            Assert.AreEqual("Vote added for choice 1 (some choice). You can change your vote by voting for something else.", result.Responses.Single().Message);
        }

        [Test]
        public void PrintsResultsWhenPollIsClosed()
        {
            var polls = new Polls();
            polls.ProcessCommand("poll start");
            polls.ProcessCommand("poll add some choice");
            polls.ProcessCommand("poll add some other choice");
            polls.ProcessCommand(new Command("channel", "user-1", "vote 1"));
            polls.ProcessCommand(new Command("channel", "user-2", "vote 1"));
            polls.ProcessCommand(new Command("channel", "user-3", "vote 2"));

            var result = polls.ProcessCommand("poll finish");
            Assert.AreEqual("Polls closed! The winner is choice 1 (some choice).\n`##` some choice\n`#` some other choice", result.Responses.Single().Message);
        }
    }
}
