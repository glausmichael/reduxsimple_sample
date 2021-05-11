using System;
using System.Windows.Input;

namespace ReduxSimple
{
    internal class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> canExecuteHandler;
        private readonly Action<object> executeAction;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action executeAction)
            : this((object param) => true, (object param) => executeAction())
        {   
        }

        public DelegateCommand(Action<object> executeAction)
            : this((object param) => true, executeAction)
        {
        }

        public DelegateCommand(Func<object, bool> canExecuteHandler, Action<object> executeAction)
        {
            this.canExecuteHandler = canExecuteHandler;
            this.executeAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecuteHandler(parameter);
        }

        public void Execute(object parameter)
        {
            this.executeAction(parameter);
        }
    }
}