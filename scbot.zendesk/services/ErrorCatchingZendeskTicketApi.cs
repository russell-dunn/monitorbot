using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace scbot.zendesk.services
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
                Trace.TraceError(e.ToString());
                return default(ZendeskTicket);
            }
        }
    }
}
