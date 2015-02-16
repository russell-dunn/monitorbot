using System;

namespace scbot.services
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