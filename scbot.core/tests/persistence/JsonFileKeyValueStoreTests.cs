using System.IO;
using NUnit.Framework;
using scbot.core.persistence;

namespace scbot.core.tests.persistence
{
    [TestFixture]
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
}