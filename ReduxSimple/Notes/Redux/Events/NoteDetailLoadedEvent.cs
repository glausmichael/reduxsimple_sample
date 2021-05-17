using ReduxSimple.Notes.Model;
using System;

namespace ReduxSimple.Sample.Notes.Redux.Events
{
    class NoteDetailLoadedEvent
    {
        public Guid NoteId { get; set; }

        public string DetailText { get; set; }
    }
}
