using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Puzzle.ViewModel
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        protected Action action;

        public RelayCommand(Action action) : this(action, true) { }

        public RelayCommand(Action action, bool canExecute)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }
}
