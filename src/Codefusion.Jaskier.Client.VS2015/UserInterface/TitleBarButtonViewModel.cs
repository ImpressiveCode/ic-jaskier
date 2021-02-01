namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using Codefusion.Jaskier.Client.VS2015.Commands;
    using Codefusion.Jaskier.Client.VS2015.Services;

    public interface ITitleBarButtonViewModel : IViewModel
    {        
        IStatusWrapper StatusWrapper { get; }

        IShowSettingsWindowCommand ShowSettingsWindowCommand { get; }

        IShowPredictionsWindowCommand ShowPredictionsWindowCommand { get; }

        IShowPredictionsPaneCommand ShowPredictionsPaneCommand { get; }

        bool PredictSavedFiles { get; set; }
    }

    public class TitleBarButtonViewModel : NotifyPropertyChangedObject, ITitleBarButtonViewModel
    {
        private readonly ISettingsStore settingsStore;

        public TitleBarButtonViewModel(
            IShowSettingsWindowCommand showSettingsWindowCommand,
            IShowPredictionsWindowCommand showPredictionsWindowCommand,
            IShowPredictionsPaneCommand showPredictionsPaneCommand,
            IStatusWrapper statusWrapper,
            ISettingsStore settingsStore)
        {
            this.StatusWrapper = statusWrapper;            
            this.ShowSettingsWindowCommand = showSettingsWindowCommand;
            this.ShowPredictionsWindowCommand = showPredictionsWindowCommand;
            this.ShowPredictionsPaneCommand = showPredictionsPaneCommand;
            this.settingsStore = settingsStore;

            this.StatusWrapper.SetWaitingForSolution();
        }

        public IStatusWrapper StatusWrapper { get; }

        public IShowSettingsWindowCommand ShowSettingsWindowCommand { get; }

        public IShowPredictionsWindowCommand ShowPredictionsWindowCommand { get; }

        public IShowPredictionsPaneCommand ShowPredictionsPaneCommand { get; }

        public bool PredictSavedFiles
        {
            get
            {
                return this.settingsStore.PredictSavedFiles;
            }

            set
            {
                if (this.settingsStore.PredictSavedFiles != value)
                {
                    this.settingsStore.PredictSavedFiles = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
