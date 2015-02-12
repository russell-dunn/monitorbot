using System.Threading.Tasks;

namespace scbot.services
{
    public interface IZendeskApi
    {
        Task<ZendeskTicket> FromId(string id);
    }
}