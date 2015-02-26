using System;
using System.Collections.Generic;

namespace scbot.core.utils
{
    public class Cache<TKey, TValue>
    {
        private class CacheEntry
        {
            public readonly TValue Item;
            public readonly DateTime Date;

            public CacheEntry(TValue item, DateTime date)
            {
                Item = item;
                Date = date;
            }
        }

        private readonly ITime m_Time;
        private readonly TimeSpan m_CacheDuration;
        private readonly IDictionary<TKey, CacheEntry> m_Cache = new Dictionary<TKey, CacheEntry>();

        public Cache(ITime time, TimeSpan cacheDuration)
        {
            m_Time = time;
            m_CacheDuration = cacheDuration;
        }

        public TValue Get(TKey key)
        {
            CacheEntry result;
            if (m_Cache.TryGetValue(key, out result) && IsNotExpired(result))
            {
                return result.Item;
            }
            return default(TValue);
        }

        private bool IsNotExpired(CacheEntry result)
        {
            return m_Time.Now - result.Date < m_CacheDuration;
        }

        public void Set(TKey key, TValue value)
        {
            m_Cache[key] = new CacheEntry(value, m_Time.Now);
        }
    }
}
