using NUnit.Framework;
using monitorbot.core.bot;
using monitorbot.core.tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.utils;

namespace monitorbot.rg.tests
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
