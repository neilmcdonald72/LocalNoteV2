using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands {
    public class DeleteCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel noteViewModel;
        private readonly Views.DeleteNoteDialog delete;

        /// Constructor
        /// <param name="noteViewModel"></param>
        public DeleteCommand(ViewModels.NoteViewModel noteViewModel) {
            this.noteViewModel = noteViewModel;
            this.delete = new Views.DeleteNoteDialog();
        }

        /// Returns true if the command can be executed. Returns false otherwise.
        /// <param name="parameter"></param>
        public bool CanExecute(object parameter) {
            // If there are no note selected or
            // if the current note hasn't been saved yet (ie. The Buffer Note)
            // Then always return false
            if (this.noteViewModel.SelectedNote == null ||
                this.noteViewModel.SelectedNote == this.noteViewModel.Buffer)
                return false;
            return true;
        }

        /// Executes the command.
        /// <param name="parameter"></param>
        public async void Execute(object parameter) {
            ContentDialogResult result = await delete.ShowAsync();

            if (result == ContentDialogResult.Primary) {
                // Get the selected note
                Models.NoteModel noteToDelete = this.noteViewModel.SelectedNote;

                // Remove the selected note from the notes lists
                this.noteViewModel.Notes.Remove(noteToDelete);
                this.noteViewModel.NotesForLV.Remove(noteToDelete);
                this.noteViewModel.FirePropertyChanged("NotesForLV");

                // Set the selected note to null
                this.noteViewModel.SelectedNote = null;

                // Reset the notes properties
                this.noteViewModel.NoteTitle = "";
                this.noteViewModel.FirePropertyChanged("NoteTitle");
                this.noteViewModel.NoteContent = new Models.ContentModel();
                this.noteViewModel.FirePropertyChanged("NoteContent");

                // Notify that the selected note has changed
                this.noteViewModel.FirePropertyChanged("SelectedNote");

                // Delete the data file in the repo
                //Repositories.NotesRepo.DeleteNoteFile(noteToDelete);

                // Delete the record from the database
                Repositories.DatabaseRepo.DeleteNote(noteToDelete);
            }
        }

        /// Fires the CanExecuteChanged event.
        public void FireCanExecuteChanged() {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
