namespace Codefusion.Jaskier.Client.VS2015.Commands
{
    using System;
    using System.Windows.Input;
    using Codefusion.Jaskier.Client.VS2015.UserInterface;

    public interface IShowSettingsWindowCommand : ICommand
    {
    }

    public class ShowSettingsWindowCommand : BaseCommand, IShowSettingsWindowCommand
    {
        private readonly Func<ISettingsWindow> settingsWindowFactory;

        public ShowSettingsWindowCommand(Func<ISettingsWindow> settingsWindowFactory)
        {
            this.settingsWindowFactory = settingsWindowFactory;
        }

        public override void Execute(object parameter)
        {
            this.settingsWindowFactory().ShowDialog();
        }
    }
}
