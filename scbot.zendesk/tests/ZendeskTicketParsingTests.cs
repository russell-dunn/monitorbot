using System.Linq;
using System.Web.Helpers;
using Moq;
using NUnit.Framework;
using scbot.zendesk.services;

namespace scbot.zendesk.tests
{
    class ZendeskTicketParsingTests
    {
        [Test]
        public void ParsesTicketFromJson()
        {
            var ticketJson = new
            {
                ticket = new { subject = "a subject", status = "open" },
            };
            var commentsJson = new
            {
                comments = new[] { new { author_id = 3, body = "a comment" } },
                count = 1,
            };
            var userJson = new
            {
                user = new { name = "bryke", photo = new { thumbnails = new[] { new { content_url = "korra" } } } },
            };

            var api = new Mock<IZendeskApi>();
            api.Setup(x => x.Ticket("1")).ReturnsAsync(FromJson(ticketJson));
            api.Setup(x => x.Comments("1")).ReturnsAsync(FromJson(commentsJson));
            api.Setup(x => x.User("3")).ReturnsAsync(FromJson(userJson));

            var ticket = new ZendeskTicketApi(api.Object).FromId("1").Result;
            Assert.AreEqual("1", ticket.Id);
            Assert.AreEqual("a subject", ticket.Description);
            Assert.AreEqual("open", ticket.Status);
            Assert.AreEqual("a comment", ticket.Comments.Single().Text);
            Assert.AreEqual("bryke", ticket.Comments.Single().Author);
            Assert.AreEqual("korra", ticket.Comments.Single().Avatar);
        }

        private object FromJson(object obj)
        {
            var json = Json.Encode(obj);
            return Json.Decode(json);
        }
    }
}
