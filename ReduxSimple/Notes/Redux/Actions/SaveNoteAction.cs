using ReduxSimple.Notes.Model;
using System;

namespace ReduxSimple.Sample.Notes.Redux.Actions
{
    class SaveNoteAction
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string DetailText { get; set; }
    }
}
