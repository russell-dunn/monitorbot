using NUnit.Framework;
using scbot.core.bot;
using scbot.core.tests;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.rg.tests
{
    class SeatingPlansTests
    {
        [Test, Explicit]
        public void CanGetResults()
        {
            var plans = new SeatingPlans(new WebClient());
            var result = plans.ProcessCommand(new Command("channel", "user", "where is mark"));
            CollectionAssert.IsNotEmpty(result.Responses);
        }
    }
}
