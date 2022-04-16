﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LocalNote.Commands {
    public class EditCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly ViewModels.NoteViewModel noteViewModel;

        /// Constructor
        /// <param name="noteViewModel"></param>
        public EditCommand(ViewModels.NoteViewModel noteViewModel) {
            this.noteViewModel = noteViewModel;
        }

        /// Returns true if the command can be executed. Returns false otherwise.
        /// <param name="parameter"></param>
        public bool CanExecute(object parameter) {
            // Always false if there are no notes selected
            if (this.noteViewModel.SelectedNote == null) return false;
            return !this.noteViewModel.EditMode;
        }

        /// Executes the command.
        /// <param name="parameter"></param>
        public void Execute(object parameter) {
            // Change the edit mode to true, then notify
            this.noteViewModel.EditMode = true;
            FireCanExecuteChanged();

            // Notify the command bar to show
            this.noteViewModel.FirePropertyChanged("EditorCommandsVisibility");

            // Change the read only mode to false, then notify
            this.noteViewModel.ReadOnly = false;
            this.noteViewModel.FirePropertyChanged("ReadOnly");

            // Enable the cancel button
            noteViewModel.CancelCommand.FireCanExecuteChanged();
        }

        /// Fires the CanExecuteChanged event.
        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
