using System;
using System.Threading.Tasks;
using scbot.core.utils;

namespace scbot.zendesk.services
{
    public class CachedZendeskApi : IZendeskApi
    {
        private readonly IZendeskApi m_Underlying;
        private readonly Cache<string, Task<dynamic>> m_TicketCache;
        private readonly Cache<string, Task<dynamic>> m_CommentsCache;
        private readonly Cache<string, Task<dynamic>> m_UserCache;

        public CachedZendeskApi(ITime time, IZendeskApi underlying)
        {
            m_Underlying = underlying;
            m_TicketCache   = new Cache<string, Task<dynamic>>(time, TimeSpan.FromMinutes(5));
            m_CommentsCache = new Cache<string, Task<dynamic>>(time, TimeSpan.FromMinutes(5));
            m_UserCache     = new Cache<string, Task<dynamic>>(time, TimeSpan.FromHours(2));
        }

        public Task<dynamic> Ticket(string ticketId)
        {
            return Cache(m_TicketCache, ticketId, () => m_Underlying.Ticket(ticketId));
        }

        public Task<dynamic> Comments(string ticketId)
        {
            return Cache(m_CommentsCache, ticketId, () => m_Underlying.Comments(ticketId));
        }

        public Task<dynamic> User(string userId)
        {
            return Cache(m_UserCache, userId, () => m_Underlying.User(userId));
        }

        private T Cache<T>(Cache<string, T> cache, string key, Func<T> valueGetter)
        {
            var cached = cache.Get(key);
            if (cached.IsDefault())
            {
                cache.Set(key, valueGetter());
            }
            return cache.Get(key);
        }
    }
}