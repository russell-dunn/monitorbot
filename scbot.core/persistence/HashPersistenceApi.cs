using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;

namespace scbot.core.persistence
{
    public class HashPersistenceApi<TValue> : IHashPersistenceApi<TValue>
    {
        private readonly IKeyValueStore m_Underlying;
        private readonly string m_UnderlyingKey;

        public HashPersistenceApi(IKeyValueStore store, string underlyingKey)
        {
            m_Underlying = store;
            m_UnderlyingKey = underlyingKey;
        }

        private IDictionary<string, TValue> ReadAll()
        {
            var json = m_Underlying.Get(m_UnderlyingKey);
            if (String.IsNullOrWhiteSpace(json)) return new Dictionary<string, TValue>();
            return Json.Decode<IDictionary<string, TValue>>(json);
        }

        public TValue Get(string key)
        {
            var dict = ReadAll();
            if (dict.ContainsKey(key)) return dict[key];
            return default(TValue);
        }

        public List<string> GetKeys()
        {
            return ReadAll().Keys.ToList();
        }

        public List<TValue> GetValues()
        {
            return ReadAll().Values.ToList();
        }

        public void Set(string key, TValue value)
        {
            var dict = ReadAll();
            dict[key] = value;
            m_Underlying.Set(m_UnderlyingKey, Json.Encode(dict));
        }
    }
}
