namespace Codefusion.Jaskier.Client.VS2015.Commands
{
    using Codefusion.Jaskier.Client.VS2015.Services;

    public interface IShowPredictionsPaneCommand : IDelegateCommand
    {        
    }

    public class ShowPredictionsPaneCommand : BaseCommand, IShowPredictionsPaneCommand
    {
        private readonly IPredictionsToolWindowController predictionsToolWindowController;

        public ShowPredictionsPaneCommand(IPredictionsToolWindowController predictionsToolWindowController)
        {
            this.predictionsToolWindowController = predictionsToolWindowController;
        }

        public override void Execute(object parameter)
        {
            this.predictionsToolWindowController.Show();
        }
    }
}
