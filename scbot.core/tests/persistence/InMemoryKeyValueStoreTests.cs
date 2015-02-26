using NUnit.Framework;
using scbot.core.persistence;

namespace scbot.core.tests.persistence
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