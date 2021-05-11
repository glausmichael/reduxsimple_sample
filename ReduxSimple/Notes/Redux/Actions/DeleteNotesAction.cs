using System;
using System.Collections.Generic;

namespace ReduxSimple.Notes
{
    class DeleteNotesAction
    {
        public IReadOnlyCollection<Guid> NoteIds { get; set; }
    }
}
