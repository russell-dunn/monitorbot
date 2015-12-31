using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot.core.utils;

namespace scbot.core.tests
{
    public class AliasListTests
    {
        [Test]
        public void CanAddAndFetchDisplayName()
        {
            var subject = new AliasList();
            subject.AddAlias("hiro.protagonist", "Hiro Protagonist", new[] { "The Deliverator", "GreatestSwordsman" });

            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("Hiro Protagonist"), 
                "should pass through display name");
            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("hiro.protagonist"),
                "should convert canonical name to display name");
            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("The Deliverator"),
                "other aliases should also work");
            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("GreatestSwordsman"),
                "more than one other aliases should also work");
            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("HIRO.PROTAGONIST"),
                "should not be case-sensitive");
            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("HIRO PROTAGONIST"), 
                "should canonicalize display name");
        }
    }
}
