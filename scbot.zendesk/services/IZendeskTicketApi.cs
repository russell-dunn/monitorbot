using System.Threading.Tasks;

namespace scbot.services.zendesk
{
    public interface IZendeskTicketApi
    {
        Task<ZendeskTicket> FromId(string id);
    }
}