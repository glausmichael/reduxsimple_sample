using ReduxSimple.Notes.Model;
using System.Collections.Generic;

namespace ReduxSimple.Notes.Redux.Events
{
    class NotesAddedEvent
    {
        public List<Note> Notes { get; internal set; }
    }
}
