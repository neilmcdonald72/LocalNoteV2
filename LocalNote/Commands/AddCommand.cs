using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands {
    public class AddCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel noteViewModel;

        /// Constructor
        /// <param name="noteViewModel"></param>
        public AddCommand(ViewModels.NoteViewModel noteViewModel) {
            this.noteViewModel = noteViewModel;
        }

        /// Returns true if the command can be executed. Returns false otherwise.
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
            // Always able to add
            return true;
        }

        /// Executes the command.
        /// <param name="parameter"></param>
        public void Execute(object parameter) {
            // Create a new note
            Models.NoteModel newNote = new Models.NoteModel();

            // Add the new note to the notes lists
            noteViewModel.Notes.Add(newNote);
            noteViewModel.NotesForLV.Add(newNote);

            // Make the new note the selected note
            // Then update the UI to go to the new note
            noteViewModel.SelectedNote = newNote;
            noteViewModel.FirePropertyChanged("SelectedNote");
        }


        /// Fires the CanExecuteChanged event.
        public void FireCanExecuteChanged() {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
