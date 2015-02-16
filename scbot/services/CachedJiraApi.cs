using System;
using System.Threading.Tasks;

namespace scbot.services
{
    public class CachedJiraApi : IJiraApi
    {
        private readonly IJiraApi m_Underlying;
        private readonly Cache<string, Task<JiraBug>> m_Cache;

        public CachedJiraApi(ITime time, IJiraApi underlying)
        {
            m_Underlying = underlying;
            m_Cache = new Cache<string, Task<JiraBug>>(time, TimeSpan.FromMinutes(5));
        }

        public Task<JiraBug> FromId(string id)
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