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
    public class SlackMessageEncodingTests
    {
        [Test]
        public void CanEncodeSimpleMessage()
        {
            var encoder = new SlackMessageEncoder();
            var json = encoder.ToJSON(new Response("Hello world", "D03JWF44C"));
            Assert.AreEqual("{\"id\":\"1\",\"type\":\"message\",\"text\":\"Hello world\",\"channel\":\"D03JWF44C\"}", json);
        }

        [Test]
        public void EncodingAMessageTwiceUsesDifferentIds()
        {
            var encoder = new SlackMessageEncoder();
            var json = encoder.ToJSON(new Response("Hello world", "D03JWF44C"));
            var json2 = encoder.ToJSON(new Response("Hello world", "D03JWF44C"));
            Assert.AreEqual("{\"id\":\"1\",\"type\":\"message\",\"text\":\"Hello world\",\"channel\":\"D03JWF44C\"}", json);
            Assert.AreEqual("{\"id\":\"2\",\"type\":\"message\",\"text\":\"Hello world\",\"channel\":\"D03JWF44C\"}", json2);
        }

        [Test]
        public void EncodingAMessageContainingAngleBracketsPreservesThem()
        {
            var encoder = new SlackMessageEncoder();
            var json = encoder.ToJSON(new Response("<http://example.com|this is a url>", "D03JWF44C"));
            Assert.AreEqual("{\"id\":\"1\",\"type\":\"message\",\"text\":\"<http://example.com|this is a url>\",\"channel\":\"D03JWF44C\"}", json);
        }
    }
}
