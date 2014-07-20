using System;
using System.Windows.Input;

namespace nhammerl.TTRecorder.ViewModel.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute)
            : this(execute, (Predicate<object>)null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
                return this._canExecute(parameter);
            else
                return true;
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }
}