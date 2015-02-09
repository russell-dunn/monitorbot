using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Moq;
using NUnit.Framework;
using scbot.slack;

namespace fasttests
{
    public class SlackMessageParsingTests
    {
        [Test]
        public void CanParseHelloMessage()
        {
            var bot = new Mock<IBot>();
            var parser = new SlackMessageHandler(bot.Object);
            parser.Handle("{ \"type\": \"hello\" }");
            bot.Verify(x => x.Hello());
        }

        [Test]
        public void CanParseUnknownMessage()
        {
            var bot = new Mock<IBot>();
            var parser = new SlackMessageHandler(bot.Object);
            var json = "{ \"type\": \"a never-before seen message type\" }";
            parser.Handle(json);
            bot.Verify(x => x.UnknownMessage(json)); 
        }
    }
}
