using ReduxSimple.Common;
using ReduxSimple.Notes.Model;
using ReduxSimple.Notes.Redux.Events;
using ReduxSimple.Redux;
using ReduxSimple.Sample.Notes.Redux.Events;
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
                .On<NoteDetailLoadedEvent>((state, @event) => {
                    var oldNote = state.Notes.FirstOrDefault(n => n.Id == @event.NoteId);
                    if (oldNote == null)
                    {
                        return state;
                    }

                    var newNote = oldNote.CopyWith(() => new Note { DetailText = @event.DetailText });
                    return state.CopyWith(() => new NotesState { Notes = state.Notes.Replace(oldNote, newNote)});
                })
                .On<NoteUpdatedEvent>((state, @event) => {
                    var oldNote = state.Notes.FirstOrDefault(n => n.Id == @event.Note.Id);
                    if (oldNote == null)
                    {
                        return state;
                    }

                    return state.CopyWith(() => new NotesState { Notes = state.Notes.Replace(oldNote, @event.Note) });
                })
                .ToList();
        }
    }
}
