using System;

namespace monitorbot.core.utils
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