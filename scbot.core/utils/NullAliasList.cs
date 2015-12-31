using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.utils
{
    public class NullAliasList : IAliasList
    {
        public string GetCanonicalNameFor(string name)
        {
            return name;
        }

        public string GetDisplayNameFor(string name)
        {
            return name;
        }
    }
}
