using ReduxSimple.Notes.Model;
using ReduxSimple.Redux;
using ReduxSimple.Sample.Notes.Redux.Actions;
using ReduxSimple.Sample.Notes.Redux.Events;
using System;
using System.Reactive.Linq;

namespace ReduxSimple.Notes
{
    class LoadNoteDetailEffect : EffectBase
    {
        private readonly ReduxStore<RootState> store;

        public LoadNoteDetailEffect(ReduxStore<RootState> store)
        {
            this.store = store;
        }

        public override Effect<RootState> CreateEffect()
        {
            return Effects.CreateEffect<RootState>(
                () => this.store.ObserveAction<LoadNoteDetailAction>()
                .Select(action =>
                {
                    return new NoteDetailLoadedEvent
                    {
                        NoteId = action.NoteId,
                        DetailText = "Some note detail"
                    };
                }), true);
        }
    }
}
