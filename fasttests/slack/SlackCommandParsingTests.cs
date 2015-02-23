using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot;
using scbot.slack;

namespace fasttests
{
    class SlackCommandParsingTests
    {
        [Test]
        public void ShouldReturnFalseWhenNotMentioned()
        {
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.False(parser.TryGetCommand(new Message("a-channel", "a-user", "something irrelevant"), out command));
            Assert.Null(command);
        }

        [TestCase("scbot")]
        [TestCase("scbot:")]
        [TestCase("  scbot  :  ")]
        public void ShouldReturnCommandWhenMentionedAtStartOfMessage(string ping)
        {
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.True(parser.TryGetCommand(new Message("a-channel", "a-user", string.Format("{0} do the thing", ping)), out command));
            Assert.AreEqual("do the thing", command);
        }

        [Test]
        public void ShouldNotReturnCommandWhenMentionedInMiddleOfMessage()
        {
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.False(parser.TryGetCommand(new Message("a-channel", "a-user", "I'm going to mention scbot in this message"), out command));
            Assert.Null(command); 
        }

        [Test]
        public void ShouldReturnCommandWhenAtMentioned()
        {
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.True(parser.TryGetCommand(new Message("a-channel", "a-user", "<@U03JWF43N> do the thing"), out command));
            Assert.AreEqual("do the thing", command); 
        }

        [Test]
        public void ShouldReturnCommandWhenItContainsNewlines()
        {
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.True(parser.TryGetCommand(new Message("a-channel", "a-user", "<@U03JWF43N> do the \n\nthing"), out command));
            Assert.AreEqual("do the \n\nthing", command);
        }

        [Test]
        public void ShouldAlwaysTreatPMsAsMentions()
        {
            // channel names that start with D are direct messages (or PMs) so the bot is always being addressed directly
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.True(parser.TryGetCommand(new Message("D03JWF44C", "a-user", "do the thing"), out command));
            Assert.AreEqual("do the thing", command); 
        }

        [Test]
        public void ShouldStillStripBotNameFromPMs()
        {
            var parser = new SlackCommandParser("scbot", "U03JWF43N");
            string command;
            Assert.True(parser.TryGetCommand(new Message("D03JWF44C", "a-user", "scbot: do the thing"), out command));
            Assert.AreEqual("do the thing", command);  
        }
        
    }
}
