using System.Threading.Tasks;

namespace scbot.services
{
    public interface IZendeskTicketApi
    {
        Task<ZendeskTicket> FromId(string id);
    }
}