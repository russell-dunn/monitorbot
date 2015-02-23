using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.services.compareengine
{
    struct Tracked<T>
    {
        public readonly T Value;
        public readonly string Channel;

        public Tracked(T value, string channel)
        {
            Value = value;
            Channel = channel;
        }
    }
}
