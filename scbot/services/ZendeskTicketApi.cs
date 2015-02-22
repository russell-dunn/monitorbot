using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace scbot.services
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
            var ticket = await m_Api.Ticket(id);
            var comments = await m_Api.Comments(id);
            return new ZendeskTicket(id, ticket.ticket.subject, ticket.ticket.status, comments.count);
        }
    }

}