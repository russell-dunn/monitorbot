using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot;
using scbot.processors;
using scbot.services;

namespace fasttests
{
    class NoteTakingTests
    {
        [Test]
        public void DelegatesToNoteApiWhenWritingNotes()
        {
            var api = new Mock<INoteApi>();
            var commandUtils = new Mock<ICommandParser>();
            var command = "note this is a note";
            var message = new Message("a-channel", "a-user", "scbot: "+command);
            commandUtils.Setup(x => x.TryGetCommand(message, out command)).Returns(true);
            api.Setup(x => x.AddNote("a-user", "this is a note")).Returns(new Note("1", "this is a note"));
            var processor = new NoteProcessor(commandUtils.Object, api.Object);
            var result = processor.ProcessMessage(message);
            Assert.AreEqual("note stored. Use `scbot delete note 1` to delete note", result.Responses.Single().Message);
            api.VerifyAll();
        }

        [Test]
        public void DelegatesToNoteApiWhenListingNotes()
        {
            var api = new Mock<INoteApi>();
            var commandUtils = new Mock<ICommandParser>();
            var command = "notes";
            var message = new Message("a-channel", "a-user", "scbot: " + command);
            commandUtils.Setup(x => x.TryGetCommand(message, out command)).Returns(true);
            api.Setup(x => x.GetNotes("a-user")).Returns(new List<Note>{new Note("1", "this is a note")});
            var processor = new NoteProcessor(commandUtils.Object, api.Object);
            var result = processor.ProcessMessage(message);
            Assert.AreEqual("1 note:\n`1` this is a note", result.Responses.Single().Message);
            api.VerifyAll(); 
        }

        [Test]
        public void DelegatesToNoteApiWhenDeletingNotes()
        {
            var api = new Mock<INoteApi>();
            var commandUtils = new Mock<ICommandParser>();
            var command = "delete note 2";
            var message = new Message("a-channel", "a-user", "scbot: " + command);
            commandUtils.Setup(x => x.TryGetCommand(message, out command)).Returns(true);
            var processor = new NoteProcessor(commandUtils.Object, api.Object);
            var result = processor.ProcessMessage(message);
            Assert.AreEqual("note 2 deleted", result.Responses.Single().Message);
            api.Verify(x => x.RemoveNote("a-user", "2"));
        }

        [Test]
        public void ReturnsUsageIfOnlyCommandIsNote()
        {
            var api = new Mock<INoteApi>();
            var commandUtils = new Mock<ICommandParser>();
            var command = "note";
            var message = new Message("a-channel", "a-user", "scbot: "+command);
            commandUtils.Setup(x => x.TryGetCommand(message, out command)).Returns(true);
            var processor = new NoteProcessor(commandUtils.Object, api.Object);
            var result = processor.ProcessMessage(message);
            Assert.AreEqual("usage: note <text> | notes | delete note <id>", result.Responses.Single().Message);
        }
    }
}
