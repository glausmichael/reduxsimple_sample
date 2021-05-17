using ReduxSimple.Notes;
using ReduxSimple.Redux;
using System.Reactive.Linq;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using ReduxSimple.Sample.Notes.Redux.Actions;
using ReduxSimple.Sample.Notes;

namespace ReduxSimple
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly IStore store;
        private IReadOnlyCollection<NoteModel> notes;

        public MainWindowViewModel(IStore store)
        {
            this.store = store;
            this.AddNoteCommand = new DelegateCommand(this.AddNote);
            this.DeleteAllNotesCommand = new DelegateCommand(this.DeleteAllNotes);
            this.DeleteNoteCommand = new DelegateCommand(this.DeleteNote);
            this.LoadNoteDetailCommand = new DelegateCommand(this.LoadNoteDetail);
            this.SaveNoteCommand = new DelegateCommand(this.SaveNote);

            this.store
                .Select(NotesSelectors.SelectNotesFeatureState)
                .Select(state => state.Notes)
                .Subscribe((notes) =>
                {
                    var noteModels = new List<NoteModel>();
                    foreach (var note in notes)
                    {
                        noteModels.Add(new NoteModel
                        {
                            Id = note.Id,
                            Text = note.Text,
                            DetailText = note.DetailText
                        });
                    }

                    Notes = noteModels;
                });
        }

        public ICommand AddNoteCommand { get; }
        public ICommand DeleteAllNotesCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand LoadNoteDetailCommand { get; }
        public ICommand SaveNoteCommand { get; }

        public IReadOnlyCollection<NoteModel> Notes
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
            this.store.Dispatch(new DeleteNotesAction { NoteIds = new List<Guid> { ((NoteModel)note).Id } });
        }

        private void LoadNoteDetail(object note)
        {
            this.store.Dispatch(new LoadNoteDetailAction { NoteId = ((NoteModel)note).Id});
        }

        private void SaveNote(object note)
        {
            var noteModel = (NoteModel)note;
            this.store.Dispatch(new SaveNoteAction { Id = noteModel.Id, Text = noteModel.Text, DetailText = noteModel.DetailText });
        }
    }
}
