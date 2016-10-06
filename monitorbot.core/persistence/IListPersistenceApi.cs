using System;
using System.Collections.Generic;

namespace monitorbot.core.persistence
{
    public interface IListPersistenceApi<T>
    {
        void AddToList(T value);
        int RemoveFromList(Predicate<T> toRemove);
        List<T> ReadList();
    }
}