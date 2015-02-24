using System;
using System.Collections.Generic;
using System.Web.Helpers;

namespace scbot.services.persistence
{
    public class ListPersistenceApi<T> : IListPersistenceApi<T>
    {
        private readonly IKeyValueStore m_Underlying;

        public ListPersistenceApi(IKeyValueStore underlying)
        {
            m_Underlying = underlying;
        }

        public void AddToList(string key, T value)
        {
            var list = ReadList(key);
            list.Add(value);
            m_Underlying.Set(key, Json.Encode(list));
        }

        public int RemoveFromList(string key, Predicate<T> toRemove)
        {
            var list = ReadList(key);
            var removed = list.RemoveAll(toRemove);
            m_Underlying.Set(key, Json.Encode(list));
            return removed;
        }

        public List<T> ReadList(string key)
        {
            var json = m_Underlying.Get(key);
            if (String.IsNullOrWhiteSpace(json)) return new List<T>();
            return Json.Decode<List<T>>(json);
        }
    }
}