using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.services
{
    public class InMemoryKeyValueStore : IKeyValueStore
    {
        private readonly Dictionary<string, string> m_Store;

        public InMemoryKeyValueStore()
        {
            m_Store = new Dictionary<string, string>();
        }

        public void Set(string key, string value)
        {
            m_Store[key] = value;
        }

        public string Get(string key)
        {
            string value;
            if (m_Store.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }
    }
}
