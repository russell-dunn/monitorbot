using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace scbot.services.zendesk
{
    public class ZendeskTicketApi : IZendeskTicketApi
    {
        private readonly IZendeskApi m_Api;

        public ZendeskTicketApi(IZendeskApi api)
        {
            m_Api = api;
        }

        public async Task<ZendeskTicket> FromId(string id)
        {
            var ticketJson = await m_Api.Ticket(id);
            var commentsJson = await m_Api.Comments(id);
            var comments = ((DynamicJsonArray)commentsJson.comments).Cast<dynamic>().Select(x => new ZendeskTicket.Comment(x.body, x.author_id.ToString(), null));
            var commentsWithAuthors = await Task.WhenAll(comments.Select(FixCommentAuthor));
            return new ZendeskTicket(id, ticketJson.ticket.subject, ticketJson.ticket.status, new List<ZendeskTicket.Comment>(commentsWithAuthors).AsReadOnly());
        }

        private async Task<ZendeskTicket.Comment> FixCommentAuthor(ZendeskTicket.Comment x)
        {
            var userJson = await m_Api.User(x.Author);
            var photo = TryGetPhoto(userJson);
            return new ZendeskTicket.Comment(x.Text, userJson.user.name, photo);
        }

        private string TryGetPhoto(dynamic userJson)
        {
            try
            {
                return userJson.user.photo.thumbnails[0].content_url;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}