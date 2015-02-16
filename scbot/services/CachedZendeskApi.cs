using System;
using System.Threading.Tasks;

namespace scbot.services
{
    public class CachedZendeskApi : IZendeskApi
    {
        private readonly IZendeskApi m_Underlying;
        private readonly Cache<string, Task<ZendeskTicket>> m_Cache;

        public CachedZendeskApi(ITime time, IZendeskApi underlying)
        {
            m_Underlying = underlying;
            m_Cache = new Cache<string, Task<ZendeskTicket>>(time, TimeSpan.FromMinutes(5));
        }

        public Task<ZendeskTicket> FromId(string id)
        {
            var cached = m_Cache.Get(id);
            if (cached == null)
            {
                m_Cache.Set(id, m_Underlying.FromId(id));
            }
            return m_Cache.Get(id);
        }
    }
}