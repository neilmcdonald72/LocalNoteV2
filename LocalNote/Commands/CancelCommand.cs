﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LocalNote.ViewModels;

namespace LocalNote.Commands {
    public class CancelCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly NoteViewModel vm;

        /// Constructor
        /// <param name="vm">The note view model.</param>
        public CancelCommand(NoteViewModel vm) {
            this.vm = vm;
        }


        /// Returns true if the command can be executed. Returns false otherwise.
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
            if (vm.EditMode) return true;
            else return false;
        }


        /// Executes the command.
        /// <param name="parameter"></param>
        public void Execute(object parameter) {
            // Notify that the edit mode value has changed
            vm.EditMode = false;
            vm.FirePropertyChanged(nameof(vm.EditMode));

            // Notify the command bar to hide
            vm.FirePropertyChanged(nameof(vm.EditorCommandsVisibility));

            // Notify that the read only value has changed
            vm.ReadOnly = true;
            vm.FirePropertyChanged(nameof(vm.ReadOnly));

            FireCanExecuteChanged();

            vm.EditCommand.FireCanExecuteChanged();
        }

        /// Firest the CanExecuteChanged event.
        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
