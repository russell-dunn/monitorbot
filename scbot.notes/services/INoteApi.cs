using System.Collections.Generic;

namespace scbot.notes.services
{
    public interface INoteApi
    {
        Note AddNote(string userId, string noteText);
        IEnumerable<Note> GetNotes(string userID);
        void RemoveNote(string userId, string noteId);
    }
}