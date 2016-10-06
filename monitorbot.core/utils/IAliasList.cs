using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.core.utils
{
    public interface IAliasList
    {
        /// <returns>Get a standard name for <paramref name="name"/> that can be used for equality tests / persistence / etc.
        /// Returns <paramref name="name" /> if no particular name found</returns>
        string GetCanonicalNameFor(string name);
        /// <returns>Get a nicer-looking display name for <paramref name="name"/> that can be used for printing out / not pinging that person / etc.
        /// Returns <paramref name="name" /> if no particular name found</returns>
        string GetDisplayNameFor(string name);
    }
}
