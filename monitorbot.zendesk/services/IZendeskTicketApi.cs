using System.Threading.Tasks;

namespace monitorbot.zendesk.services
{
    public interface IZendeskTicketApi
    {
        Task<ZendeskTicket> FromId(string id);
    }
}