namespace Codefusion.Jaskier.Client.VS2015.Commands
{
    using System;
    using Codefusion.Jaskier.Client.VS2015.UserInterface;

    public interface IShowPredictionsWindowCommand : IDelegateCommand
    {        
    }

    public class ShowPredictionsWindowCommand : BaseCommand, IShowPredictionsWindowCommand
    {
        private readonly Func<IPredictionsWindow> factory;

        public ShowPredictionsWindowCommand(Func<IPredictionsWindow> factory)
        {
            this.factory = factory;
        }

        public override void Execute(object parameter)
        {
            this.factory().ShowDialog();
        }    
    }
}
