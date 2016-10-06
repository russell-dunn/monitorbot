using System.Collections.Generic;

namespace monitorbot.core.persistence
{
    public interface IHashPersistenceApi<TValue>
    {
        List<string> GetKeys();
        List<TValue> GetValues();
        TValue Get(string key);
        void Set(string key, TValue value);
    }
}
