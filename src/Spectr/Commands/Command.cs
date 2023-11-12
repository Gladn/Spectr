using System;
using System.Windows.Input;

namespace Spectr.Commands
{
    /// <summary>
    /// Базовый класс Command
    /// </summary>
    internal abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}
