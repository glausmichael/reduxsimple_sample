using System;

namespace ReduxSimple.Notes.Model
{
    class Note
    {
        public Guid Id { get; set; }
        public string Text { get; internal set; }
    }
}
