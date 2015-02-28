using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.persistence
{
    public interface IHashPersistenceApi<TValue>
    {
        List<string> GetKeys();
        List<TValue> GetValues();
        TValue Get(string key);
        void Set(string key, TValue value);
    }
}
