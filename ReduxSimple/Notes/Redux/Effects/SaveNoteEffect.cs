using Converto;
using ReduxSimple.Notes.Model;
using ReduxSimple.Notes.Redux.Events;
using ReduxSimple.Redux;
using ReduxSimple.Sample.Notes.Redux.Actions;
using ReduxSimple.Sample.Notes.Redux.Events;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ReduxSimple.Notes
{
    class SaveNoteEffect : EffectBase
    {
        private readonly ReduxStore<RootState> store;

        public SaveNoteEffect(ReduxStore<RootState> store)
        {
            this.store = store;
        }

        public override Effect<RootState> CreateEffect()
        {
            return Effects.CreateEffect<RootState>(
                () => this.store.ObserveAction<SaveNoteAction>()
                .Select(action =>
                {
                    return new NoteUpdatedEvent
                    {
                        Note = new Note
                        {
                            Id = action.Id,
                            Text = action.Text,
                            DetailText = action.DetailText
                        }
                    };
                }), true);
        }
    }
}
