using System;
using System.Collections.Generic;

namespace scbot.core.persistence
{
    public interface IListPersistenceApi<T>
    {
        void AddToList(T value);
        int RemoveFromList(Predicate<T> toRemove);
        List<T> ReadList();
    }
}