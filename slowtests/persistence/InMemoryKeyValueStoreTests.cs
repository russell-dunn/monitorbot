using NUnit.Framework;
using scbot.services.persistence;

namespace slowtests.persistence
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