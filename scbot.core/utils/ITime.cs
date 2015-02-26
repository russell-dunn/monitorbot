using System;

namespace scbot.services
{
    public interface ITime
    {
        DateTime Now { get; }
    }
}