using ReduxSimple.Notes.Redux.Events;
using ReduxSimple.Redux;
using System.Reactive.Linq;

namespace ReduxSimple.Notes
{
    class DeleteNotesEffect : EffectBase
    {
        private readonly ReduxStore<RootState> store;

        public DeleteNotesEffect(ReduxStore<RootState> store)
        {
            this.store = store;
        }

        public override Effect<RootState> CreateEffect()
        {
            return Effects.CreateEffect<RootState>(
                () => this.store.ObserveAction<DeleteNotesAction>()
                .Select(action =>
                {
                    return new NotesDeletedEvent
                    {
                        DeletedNoteIds = action.NoteIds
                    };
                }), true);
        }
    }
}
