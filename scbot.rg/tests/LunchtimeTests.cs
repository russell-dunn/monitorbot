using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.rg.tests
{
    class LunchtimeTests
    {
        [Test]
        public void IsNearlyLunchtime()
        {
            Assert.AreEqual("It's nearly lunchtime!", Webcams.LunchMessage(TimeSpan.FromHours(11.5)));
        }

        [Test]
        public void IsActuallyLunchtime()
        {
            Assert.AreEqual("Lunchtime!", Webcams.LunchMessage(TimeSpan.FromHours(12.5)));
        }
    }
}
