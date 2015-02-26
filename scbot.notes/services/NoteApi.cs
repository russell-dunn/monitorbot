using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;
using scbot.core.persistence;

namespace scbot.notes.services
{
    public class NoteApi : INoteApi
    {
        private readonly IKeyValueStore m_KeyValueStore;

        public NoteApi(IKeyValueStore keyValueStore)
        {
            m_KeyValueStore = keyValueStore;
        }

        public Note AddNote(string userId, string noteText)
        {
            var counter = m_KeyValueStore.Get(KeyForNextNoteId(userId)) ?? "1";
            var newNote = new Note(counter, noteText);
            var notesList = GetNotesList(userId) ?? new List<Note>();
            notesList.Add(newNote);

            SetNotesList(userId, notesList);
            m_KeyValueStore.Set(KeyForNextNoteId(userId), (Int32.Parse(counter) + 1).ToString(CultureInfo.InvariantCulture));

            return newNote;
        }

        private static string KeyForNextNoteId(string userId)
        {
            return "notes:" + userId + ":next";
        }

        private static string KeyForNotes(string userId)
        {
            return "notes:" + userId;
        }

        private void SetNotesList(string userId, List<Note> notesList)
        {
            m_KeyValueStore.Set(KeyForNotes(userId), Json.Encode(notesList));
        }

        private List<Note> GetNotesList(string userId)
        {
            var notesListJson = m_KeyValueStore.Get(KeyForNotes(userId));
            if (notesListJson != null)
            {
               return Json.Decode<List<Note>>(notesListJson);
            }
            return null;
        }

        public IEnumerable<Note> GetNotes(string userID)
        {
            var notesListJson = m_KeyValueStore.Get(KeyForNotes(userID));
            if (notesListJson == null) return Enumerable.Empty<Note>();
            return Json.Decode<List<Note>>(notesListJson);
        }

        public void RemoveNote(string userId, string noteId)
        {
            var notes = GetNotesList(userId);
            notes.RemoveAll(x => x.Id == noteId);
            SetNotesList(userId, notes);
        }
    }
}