using ReduxSimple.Common;
using ReduxSimple.Notes.Redux.Events;
using ReduxSimple.Redux;
using System.Collections.Generic;
using System.Linq;

namespace ReduxSimple.Notes
{
    class NotesReducers : IReducerProvider
    {
        public IEnumerable<On<RootState>> GetReducers()
        {
            return Reducers.CreateSubReducers(NotesSelectors.SelectNotesFeatureState, (rootState, featureState) => rootState.CopyWith(featureState))
                .On<NotesAddedEvent>((state, @event) => {
                    return state.CopyWith(() => new NotesState { Notes = state.Notes.AddRange(@event.Notes) });
                })
                .On<NotesDeletedEvent>((state, @event) => {
                    return state.CopyWith(() => new NotesState { Notes = state.Notes.RemoveAll((note) => @event.DeletedNoteIds.Any(id => id == note.Id)) });
                })
                .ToList();
        }
    }
}
