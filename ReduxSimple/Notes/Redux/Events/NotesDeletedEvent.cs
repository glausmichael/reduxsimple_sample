using System;
using System.Collections.Generic;

namespace ReduxSimple.Notes.Redux.Events
{
    class NotesDeletedEvent
    {
        public IReadOnlyCollection<Guid> DeletedNoteIds { get; set; }
    }
}
