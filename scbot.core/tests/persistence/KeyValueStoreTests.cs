using System.Linq;
using NUnit.Framework;
using scbot.services.persistence;

namespace slowtests.persistence
{
    abstract class KeyValueStoreTests
    {
        private IKeyValueStore m_KeyValueStore;

        protected abstract IKeyValueStore Create();
        protected abstract void Cleanup(IKeyValueStore store);

        [SetUp]
        public void SetUp()
        {
            m_KeyValueStore = Create();
        }

        [TearDown]
        public void TearDown()
        {
            Cleanup(m_KeyValueStore);
        }

        [Test]
        public void DefaultValueIsNull()
        {
            Assert.Null(m_KeyValueStore.Get("a-key"));
        }
        
        [Test]
        public void CanStoreValue()
        {
            m_KeyValueStore.Set("a-key", "a-value");
            Assert.AreEqual("a-value", m_KeyValueStore.Get("a-key"));
        }

        [Test]
        public void CanStoreValueTwice()
        {
            m_KeyValueStore.Set("a-key", "a-value");
            m_KeyValueStore.Set("a-key", "a-value2");
            Assert.AreEqual("a-value2", m_KeyValueStore.Get("a-key"));
        }

        [Test]
        public void CanStoreLotsOfChars()
        {
            var chars = new string(Enumerable.Range(0, 256).Select(x => (char)x).ToArray());
            m_KeyValueStore.Set("a-key", chars);
            Assert.AreEqual(chars, m_KeyValueStore.Get("a-key"));
        }
    }
}
