using Moq;
using NUnit.Framework;
using scbot.core.persistence;
using scbot.notes.services;

namespace scbot.notes.tests
{
    class NoteApiTests
    {
        [Test]
        public void NoteApiUsesKVStoreToStoreNotes()
        {
            var kvStore = new Mock<IKeyValueStore>();
            var noteApi = new NoteApi(kvStore.Object);

            noteApi.AddNote("user-id", "this is a note");

            kvStore.Verify(x => x.Set("notes:user-id", "[{\"Id\":\"1\",\"Text\":\"this is a note\"}]"));
            kvStore.Verify(x => x.Set("notes:user-id:next", "2"));
        }

        [Test]
        public void NoteApiUsesKVStoreToStoreSecondNote()
        {
            var kvStore = new Mock<IKeyValueStore>();
            var noteApi = new NoteApi(kvStore.Object);

            kvStore.Setup(x => x.Get("notes:user-id:next")).Returns("2");
            kvStore.Setup(x => x.Get("notes:user-id")).Returns("[{\"Id\":\"1\",\"Text\":\"this is a note\"}]");

            noteApi.AddNote("user-id", "this is another note");

            kvStore.Verify(x => x.Set("notes:user-id", "[{\"Id\":\"1\",\"Text\":\"this is a note\"},{\"Id\":\"2\",\"Text\":\"this is another note\"}]"));
            kvStore.Verify(x => x.Set("notes:user-id:next", "3"));
        }

        [Test]
        public void NoteApiUsesKVStoreToListNotes()
        {
            var kvStore = new Mock<IKeyValueStore>();
            var noteApi = new NoteApi(kvStore.Object);

            kvStore.Setup(x => x.Get("notes:user-id")).Returns("[{\"Id\":\"1\",\"Text\":\"this is a note\"},{\"Id\":\"2\",\"Text\":\"this is another note\"}]");
            kvStore.Setup(x => x.Get("notes:user-id:next")).Returns("3");

            var notes = noteApi.GetNotes("user-id");
            CollectionAssert.AreEqual(new[] {new Note("1", "this is a note"), new Note("2", "this is another note")}, notes);
        }

        [Test]
        public void UsesKVStoreToDeleteNotes()
        {
            var kvStore = new Mock<IKeyValueStore>();
            var noteApi = new NoteApi(kvStore.Object);

            kvStore.Setup(x => x.Get("notes:user-id")).Returns("[{\"Id\":\"1\",\"Text\":\"this is a note\"},{\"Id\":\"2\",\"Text\":\"this is another note\"}]");
            kvStore.Setup(x => x.Get("notes:user-id:next")).Returns("3");

            noteApi.RemoveNote("user-id", "1");
            kvStore.Verify(x => x.Set("notes:user-id", "[{\"Id\":\"2\",\"Text\":\"this is another note\"}]"));
        }

        // TODO: update notes
        // TODO: fail sensibly if specified note not found
    }
}
