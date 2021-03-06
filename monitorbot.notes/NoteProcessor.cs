using System;
using System.Collections.Generic;
using System.Linq;
using monitorbot.core.bot;
using monitorbot.core.persistence;
using monitorbot.notes.services;
using monitorbot.core.utils;

namespace monitorbot.notes
{
    public class NoteProcessor : IMessageProcessor
    {
        public static IFeature Create(ICommandParser commandParser, IKeyValueStore persistence)
        {
            return new BasicFeature("notes", "save notes for later", "use `note <text>` to save a note, `notes` to list notes and `delete note <num>` to delete a specific note",
                new NoteProcessor(commandParser, new NoteApi(persistence)));
        }

        private readonly ICommandParser m_CommandParser;
        private readonly INoteApi m_NoteApi;

        public NoteProcessor(ICommandParser commandParser, INoteApi noteApi)
        {
            m_CommandParser = commandParser;
            m_NoteApi = noteApi;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            string noteText;
            string ignored;
            string noteId;
            if (m_CommandParser.TryGetCommand(message, "note", out noteText))
            {
                if (String.IsNullOrWhiteSpace(noteText)) return Usage(message);
                var note = m_NoteApi.AddNote(message.User, noteText);
                return Response.ToMessage(message, FormatNoteStoredResponse(note));
            }
            if (m_CommandParser.TryGetCommand(message, "notes", out ignored))
            {
                var notes = m_NoteApi.GetNotes(message.User);
                return Response.ToMessage(message, FormateNoteListResponse(notes.ToList()));
            }
            if (m_CommandParser.TryGetCommand(message, "delete note", out noteId))
            {
                m_NoteApi.RemoveNote(message.User, noteId);
                return Response.ToMessage(message, "note "+noteId+" deleted");
            }
            return MessageResult.Empty;
        }

        private static MessageResult Usage(Message message)
        {
            return Response.ToMessage(message, "usage: note <text> | notes | delete note <id>");
        }

        private static string FormatNoteStoredResponse(Note note)
        {
            return string.Format("note stored. Use `delete note {0}` to delete note", note.Id);
        }

        private static string FormateNoteListResponse(List<Note> notes)
        {
            return string.Format("{0} note{1}:\n", notes.Count, (notes.Count != 1 ? "s" : "")) +
                String.Join("\n", notes.Select(x => string.Format("`{0}` {1}", x.Id, x.Text)));
        }
    }
}