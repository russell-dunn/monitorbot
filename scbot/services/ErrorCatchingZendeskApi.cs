using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.services
{
    public class ErrorCatchingZendeskApi : IZendeskApi
    {
        private readonly IZendeskApi m_Underlying;

        public ErrorCatchingZendeskApi(IZendeskApi underlying)
        {
            m_Underlying = underlying;
        }

        public async Task<ZendeskTicket> FromId(string id)
        {
            try
            {
                return await m_Underlying.FromId(id);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n" + e + "\n\n" + DateTime.Now);
                return default(ZendeskTicket); // TODO: log
            }
        }
    }
}
