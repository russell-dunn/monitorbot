using System;

namespace scbot.core.utils
{
    public class Time : ITime
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