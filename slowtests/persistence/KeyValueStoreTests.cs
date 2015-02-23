using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot.services;

namespace slowtests
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

    class JsonFileKeyValueStoreTests : KeyValueStoreTests
    {
        private FileInfo m_File;

        protected override IKeyValueStore Create()
        {
            Assert.Null(m_File);
            m_File = new FileInfo(Path.GetTempFileName());
            return new JsonFileKeyValueStore(m_File);
        }

        protected override void Cleanup(IKeyValueStore store)
        {
            Assert.NotNull(m_File);
            m_File.Delete();
            m_File = null;
        }
    }

    class InMemoryKeyValueStoreTests : KeyValueStoreTests
    {
        protected override IKeyValueStore Create()
        {
            return new InMemoryKeyValueStore();
        }

        protected override void Cleanup(IKeyValueStore store)
        {
        }
    }
}
