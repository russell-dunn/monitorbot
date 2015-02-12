using System.Collections.Generic;

namespace scbot.services
{
    public interface INoteApi
    {
        Note AddNote(string userId, string noteText); // TODO: disallow duplicate notes?
        IEnumerable<Note> GetNotes(string userID);
        void RemoveNote(string userId, string noteId); // TODO: return success if note found?
    }
}