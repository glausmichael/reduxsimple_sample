using System;

namespace ReduxSimple.Notes.Model
{
    class Note
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string DetailText { get; set; }
    }
}
