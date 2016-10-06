using System.Threading.Tasks;

namespace monitorbot.zendesk.services
{
    public interface IZendeskApi
    {
        Task<dynamic> Ticket(string ticketId);
        Task<dynamic> Comments(string ticketId);
        Task<dynamic> User(string userId);
    }
}
