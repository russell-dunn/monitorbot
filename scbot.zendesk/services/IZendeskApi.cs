using System.Threading.Tasks;

namespace scbot.services.zendesk
{
    public interface IZendeskApi
    {
        Task<dynamic> Ticket(string ticketId);
        Task<dynamic> Comments(string ticketId);
        Task<dynamic> User(string userId);
    }
}
