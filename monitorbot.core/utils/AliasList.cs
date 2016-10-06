using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.core.utils
{
    public class AliasList : IAliasList
    {
        // there's actually two symmetric classes here -
        // could easily dedupe into one nested class, but not sure what to call it

        private readonly Dictionary<string, string> m_NamesToDisplayName
            = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, string> m_NamesToCanonicalName
            = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public string GetCanonicalNameFor(string name)
        {
            string canonicalName;
            if (m_NamesToCanonicalName.TryGetValue(name, out canonicalName))
            {
                return canonicalName;
            }
            return name;
        }

        public string GetDisplayNameFor(string name)
        {
            string displayName;
            if (m_NamesToDisplayName.TryGetValue(name, out displayName))
            {
                return displayName;
            }
            return name;
        }

        public void AddAlias(string canonicalName, string displayName, IEnumerable<string> otherAliases)
        {
            m_NamesToDisplayName[canonicalName] = displayName;
            m_NamesToDisplayName[displayName] = displayName;
            foreach (var otherAlias in otherAliases)
            {
                m_NamesToDisplayName[otherAlias] = displayName;
            }

            m_NamesToCanonicalName[canonicalName] = canonicalName;
            m_NamesToCanonicalName[displayName] = canonicalName;
            foreach (var otherAlias in otherAliases)
            {
                m_NamesToCanonicalName[otherAlias] = canonicalName;
            }
        }
    }
}
