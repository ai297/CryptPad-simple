using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CryptoPad.ViewModels
{
    public class RelayCommand : ICommand
    {
        private Action<object> command;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke(parameter) ?? false;
        }

        public void Execute(object parameter)
        {
            this.command?.Invoke(parameter);
        }

        public RelayCommand(Action<object> command, Func<object, bool> canExecute)
        {
            this.command = command;
            this.canExecute = canExecute;
        }
    }
}
