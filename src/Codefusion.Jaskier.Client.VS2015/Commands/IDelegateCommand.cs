namespace Codefusion.Jaskier.Client.VS2015.Commands
{
    using System.Windows.Input;

    public interface IDelegateCommand : ICommand
    {
        void InvalidateCanExecute();
    }
}
