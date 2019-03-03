using System;
using System.Windows.Input;

namespace SelectByParameters.Lib.Mvvm
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> action;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> action, Func<object, bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                action(parameter);
            }
            catch (Exception ex)
            {
                ex.Message.WriteLine();
            }
        }
    }
}
