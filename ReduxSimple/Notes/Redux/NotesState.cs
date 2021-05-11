using ReduxSimple.Notes.Model;
using ReduxSimple.Redux;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ReduxSimple.Notes
{
    class NotesState : IMainFeatureState
    {
        public ImmutableList<Note> Notes { get; set; } = new List<Note>().ToImmutableList();
    }
}
