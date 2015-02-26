using System.Threading.Tasks;

namespace scbot.zendesk.services
{
    public interface IZendeskTicketApi
    {
        Task<ZendeskTicket> FromId(string id);
    }
}