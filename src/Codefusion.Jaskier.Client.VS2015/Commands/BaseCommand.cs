namespace Codefusion.Jaskier.Client.VS2015.Commands
{
    using System;

    public class BaseCommand : IDelegateCommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public BaseCommand()
            : this(null)
        {            
        }

        public BaseCommand(Action<object> execute)
            : this(execute, null)
        {            
        }

        public BaseCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke(parameter) ?? true;
        }

        public virtual void Execute(object parameter)
        {
            this.execute?.Invoke(parameter);
        }

        public void InvalidateCanExecute()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
