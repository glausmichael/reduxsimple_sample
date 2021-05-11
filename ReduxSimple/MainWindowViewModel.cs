using ReduxSimple.Notes;
using ReduxSimple.Redux;
using System.Reactive.Linq;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using ReduxSimple.Notes.Model;
using System.Linq;

namespace ReduxSimple
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly IStore store;
        private IReadOnlyCollection<Note> notes;

        public MainWindowViewModel(IStore store)
        {
            this.store = store;
            this.AddNoteCommand = new DelegateCommand(this.AddNote);
            this.DeleteAllNotesCommand = new DelegateCommand(this.DeleteAllNotes);
            this.DeleteNoteCommand = new DelegateCommand(this.DeleteNote);
            this.store
                .Select(NotesSelectors.SelectNotesFeatureState)
                .Select(state => state.Notes)
                .Subscribe((notes) => Notes = notes);
        }

        public ICommand AddNoteCommand { get; }
        public ICommand DeleteAllNotesCommand { get; }
        public ICommand DeleteNoteCommand { get; }

        public IReadOnlyCollection<Note> Notes
        {
            get => notes;
            set
            {
                notes = value;
                this.OnPropertyChanged();
            }
        }

        private void AddNote()
        {
            this.store.Dispatch(new AddNoteAction { Text = "Some note text" });
        }

        private void DeleteAllNotes()
        {
            this.store.Dispatch(new DeleteNotesAction { NoteIds = this.Notes.Select(n => n.Id).ToList() });
        }

        private void DeleteNote(object note)
        {
            this.store.Dispatch(new DeleteNotesAction { NoteIds = new List<Guid> { ((Note)note).Id } });
        }
    }
}
