using scbot.services;

namespace slowtests
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