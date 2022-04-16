using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocalNote.Commands {
    public class AboutCommand : ICommand {
        public event EventHandler CanExecuteChanged;
        private readonly Page page;

        /// Constructor
        public AboutCommand(Page page) {
            this.page = page;
        }

        /// Always returns true.
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) {
            return true;
        }


        /// Executes the about command.
        /// <param name="parameter"></param>
        public void Execute(object parameter) {
            page.Frame.Navigate(typeof(AboutPage));
        }


        /// Firest the CanExecuteChanged event.
        public void FireCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
