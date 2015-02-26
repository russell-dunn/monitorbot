using System;
using System.Collections.Generic;

namespace scbot.core.persistence
{
    public interface IListPersistenceApi<T>
    {
        void AddToList(string key, T value);
        int RemoveFromList(string key, Predicate<T> toRemove);
        List<T> ReadList(string key);
    }
}