﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.slack;

namespace fasttests
{
    public class SlackMessageParsingTests
    {
        private Mock<IBot> m_Mock;

        [SetUp]
        public void SetUp()
        {
            m_Mock = new Mock<IBot>();
            m_Mock.Setup(x => x.Hello()).Returns(MessageResult.Empty);
            m_Mock.Setup(x => x.Unknown(It.IsAny<string>())).Returns(MessageResult.Empty);
            m_Mock.Setup(x => x.Message(It.IsAny<Message>())).Returns(MessageResult.Empty);
        }

        [Test]
        public void CanParseHelloMessage()
        {
            var bot = m_Mock;
            var parser = new SlackMessageHandler(bot.Object, "user-id");
            parser.Handle("{ \"type\": \"hello\" }");
            bot.Verify(x => x.Hello());
        }

        [Test]
        public void CanParseUnknownMessage()
        {
            var bot = m_Mock;
            var parser = new SlackMessageHandler(bot.Object, "user-id");
            var json = "{ \"type\": \"a never-before seen message type\" }";
            parser.Handle(json);
            bot.Verify(x => x.Unknown(json)); 
        }

        [Test]
        public void CanParseBasicMessage()
        {
            var bot = m_Mock;
            var parser = new SlackMessageHandler(bot.Object, "user-id");
            var json = "{\"type\":\"message\",\"channel\":\"D03JWF44C\",\"user\":\"U03JU40UP\",\"text\":\"this is a test\",\"ts\":\"1423514301.000002\",\"team\":\"T03JU3JV5\"}";
            parser.Handle(json);
            bot.Verify(x => x.Message(new Message("D03JWF44C", "U03JU40UP", "this is a test")));
        }

        [Test]
        public void IgnoresMessageFromSelf()
        {
            var bot = m_Mock;
            var parser = new SlackMessageHandler(bot.Object, "bot-id");
            parser.Handle("{\"type\":\"message\",\"channel\":\"D03JWF44C\",\"user\":\"bot-id\",\"text\":\"this is a test\",\"ts\":\"1423514301.000002\",\"team\":\"T03JU3JV5\"}");
            bot.Verify(x => x.Message(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public void IgnoresMessagesFromBots()
        {
            var bot = m_Mock;
            var parser = new SlackMessageHandler(bot.Object, "bot-id");
            parser.Handle("{\"type\":\"message\",\"subtype\":\"bot_message\",\"channel\":\"D03JWF44C\",\"user\":\"U03JU40UP\",\"text\":\"this is a test\",\"ts\":\"1423514301.000002\",\"team\":\"T03JU3JV5\"}");
            bot.Verify(x => x.Message(It.IsAny<Message>()), Times.Never); 
        }
    }
}
