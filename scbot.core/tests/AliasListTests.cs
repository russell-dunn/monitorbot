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
            subject.AddAlias("hiro.protagonist", "Hiro Protagonist", new[] { "The Deliverator", "GreatestSwordsman " });

            Assert.AreEqual("Hiro Protagonist", subject.GetDisplayNameFor("Hiro Protagonist"));
        }
    }
}
