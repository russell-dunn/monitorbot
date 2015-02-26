﻿namespace scbot.core.compareengine
{
    public struct Tracked<T>
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
