using System;

namespace scbot.services
{
    class Time : ITime
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}