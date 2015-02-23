using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.services
{
    public interface IZendeskApi
    {
        Task<dynamic> Ticket(string ticketId);
        Task<dynamic> Comments(string ticketId);
        Task<dynamic> User(string userId);
    }
}
