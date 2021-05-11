using ReduxSimple.Notes.Model;
using ReduxSimple.Notes.Redux.Events;
using ReduxSimple.Redux;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ReduxSimple.Notes
{
    class AddNotesEffect : EffectBase
    {
        private readonly ReduxStore<RootState> store;

        public AddNotesEffect(ReduxStore<RootState> store)
        {
            this.store = store;
        }

        public override Effect<RootState> CreateEffect()
        {
            return Effects.CreateEffect<RootState>(
                () => this.store.ObserveAction<AddNoteAction>()
                .Select(action =>
                {
                    return new NotesAddedEvent
                    {
                        Notes = new List<Note>
                        {
                            new Note
                            {
                                Id = Guid.NewGuid(),
                                Text = action.Text
                            }
                        }
                    };
                }), true);
        }
    }
}
