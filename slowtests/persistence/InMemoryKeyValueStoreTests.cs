using scbot.services.persistence;

namespace slowtests.persistence
{
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