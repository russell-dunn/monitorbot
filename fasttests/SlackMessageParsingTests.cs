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
            parser.Handle(CreateMessage("{ \"type\": \"hello\" }"));
            bot.Verify(x => x.Hello());
        }

        private dynamic CreateMessage(string json)
        {
            return Json.Decode(json);
        }
    }
}
