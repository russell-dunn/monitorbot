using monitorbot.core.persistence;
using NUnit.Framework;

namespace monitorbot.core.tests.persistence
{
    [TestFixture]
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