using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.utils
{
    public class AliasList : IAliasList
    {
        private readonly Dictionary<string, string> m_NamesToDisplayName
            = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public string GetCanonicalNameFor(string name)
        {
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
            m_NamesToDisplayName.Add(canonicalName, displayName);
            m_NamesToDisplayName.Add(displayName, displayName);
            foreach (var otherAlias in otherAliases)
            {
                m_NamesToDisplayName.Add(otherAlias, displayName);
            }
        }
    }
}
