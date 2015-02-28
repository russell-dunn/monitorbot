using System;
using System.Collections.Generic;
using System.Web.Helpers;

namespace scbot.core.persistence
{
    public class ListPersistenceApi<T> : IListPersistenceApi<T>
    {
        private readonly IKeyValueStore m_Underlying;
        private readonly string m_Key;

        public ListPersistenceApi(IKeyValueStore underlying, string key)
        {
            m_Underlying = underlying;
            m_Key = key;
        }

        public void AddToList(T value)
        {
            var list = ReadList();
            list.Add(value);
            m_Underlying.Set(m_Key, Json.Encode(list));
        }

        public int RemoveFromList(Predicate<T> toRemove)
        {
            var list = ReadList();
            var removed = list.RemoveAll(toRemove);
            m_Underlying.Set(m_Key, Json.Encode(list));
            return removed;
        }

        public List<T> ReadList()
        {
            var json = m_Underlying.Get(m_Key);
            if (String.IsNullOrWhiteSpace(json)) return new List<T>();
            return Json.Decode<List<T>>(json);
        }
    }
}