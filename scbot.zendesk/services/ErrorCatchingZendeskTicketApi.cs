using System;
using System.Threading.Tasks;

namespace scbot.services.zendesk
{
    public class ErrorCatchingZendeskTicketApi : IZendeskTicketApi
    {
        private readonly IZendeskTicketApi m_Underlying;

        public ErrorCatchingZendeskTicketApi(IZendeskTicketApi underlying)
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
