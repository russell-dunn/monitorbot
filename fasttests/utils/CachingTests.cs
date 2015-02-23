using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.services;

namespace fasttests
{
    class CachingTests
    {
        private readonly Cache<string, string> m_Cache;
        private readonly Mock<ITime> m_Time;

        public CachingTests()
        {
            m_Time = new Mock<ITime>();
            m_Cache = new Cache<string, string>(m_Time.Object, TimeSpan.FromMinutes(5));
        }

        [Test]
        public void ReturnsDefaultValueWhenValueNotCached()
        {
            Assert.Null(m_Cache.Get("foo"));
        }

        [Test]
        public void ReturnsSetValueWhenValueSet()
        {
            m_Cache.Set("foo", "bar");
            Assert.AreEqual("bar", m_Cache.Get("foo"));
        }

        [Test]
        public void ReturnsDefaultValueWhenValueIsOlderThanCacheDuration()
        {
            m_Time.Setup(x => x.Now).Returns(DateTime.Now);
            m_Cache.Set("foo", "bar");
            m_Time.Setup(x => x.Now).Returns(DateTime.Now.AddMinutes(10));

            Assert.Null(m_Cache.Get("foo"));
        }
    }
}
