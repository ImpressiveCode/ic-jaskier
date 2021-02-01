namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    using Codefusion.Jaskier.Client.VS2015.Commands;
    using Codefusion.Jaskier.Client.VS2015.Services;
    using Codefusion.Jaskier.Common.Services;

    public interface IPredictionsViewModel
    {
        IEnumerable<PredictionViewModel> Predictions { get; }

        IDelegateCommand GoToFileCommand { get; }

        IDelegateCommand RefreshCommand { get; }

        bool NoContent { get; }

        string NoContentText { get; }

        bool IsBusy { get; }
    }

    public class PredictionsViewModel : NotifyPropertyChangedObject, IPredictionsViewModel
    {
        private readonly ObservableCollection<PredictionViewModel> predictions;
        private readonly ICachedPredictionService predictionService;
        private readonly IErrorHandler errorHandler;
        private readonly IVsBridge vsBridge;
        private readonly IStatusWrapper statusWrapper;

        private bool noContent;
        private string noContentText;
        private bool isBusy;

        public PredictionsViewModel(
            ICachedPredictionService predictionService,
            IErrorHandler errorHandler,
            IVsBridge vsBridge,
            IStatusWrapper statusWrapper)
        {
            this.predictionService = predictionService;
            this.predictions = new ObservableCollection<PredictionViewModel>();
            this.errorHandler = errorHandler;
            this.vsBridge = vsBridge;
            this.statusWrapper = statusWrapper;

            this.RefreshCommand = new BaseCommand(this.Refresh, this.CanRefresh);
            this.GoToFileCommand = new BaseCommand(this.GoToFile);

            this.NoContent = true;
            this.NoContentText = Strings.PleaseClickRefreshButtonToLoadPredictions;

            this.predictionService.PredictionsLoading += this.OnPredictionsLoading;
            this.predictionService.PredictionsLoaded += this.OnPredictionsLoaded;            
        }        

        public IEnumerable<PredictionViewModel> Predictions => this.predictions;

        public IDelegateCommand GoToFileCommand { get; }

        public IDelegateCommand RefreshCommand { get; }

        public bool NoContent
        {
            get
            {
                return this.noContent;
            }

            set
            {
                if (this.noContent != value)
                {
                    this.noContent = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string NoContentText
        {
            get
            {
                return this.noContentText;
            }

            set
            {
                if (this.noContentText != value)
                {
                    this.noContentText = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.OnPropertyChanged();

                    this.InvalidateCommands();
                }
            }
        }

        private void GoToFile(object o)
        {
            var prediction = (PredictionViewModel)o;

            if (!string.IsNullOrEmpty(prediction.Path))
            {
                this.vsBridge.OpenSolutionDocument(prediction.Path);
            }
        }

        private bool CanRefresh(object arg)
        {
            return !this.isBusy;
        }

        private void Refresh(object obj)
        {
            this.predictionService.Reload();
        }    

        private void InvalidateCommands()
        {
            this.RefreshCommand.InvalidateCanExecute();
        }

        private void OnPredictionsLoading(object sender, EventArgs e)
        {
            this.IsBusy = true;
            this.NoContent = true;
            this.NoContentText = Strings.Loading;
        }

        private void OnPredictionsLoaded(object sender, EventArgs e)
        {
            try
            {
                this.predictions.Clear();

                if (string.IsNullOrWhiteSpace(this.vsBridge.SolutionFileName))
                {
                    this.statusWrapper.SetReady();
                    this.NoContentText = Strings.StringSolutionNotAvailable;
                    return;
                }

                var result = this.predictionService.GetCurrentPredictions();
                if (result == null)
                {
                    return;
                }

                if (result.NumberOfChangedFiles == 0)
                {
                    this.statusWrapper.SetReady();
                    this.NoContentText = Strings.NoFilesChanged;
                    return;
                }

                if (result.PredictionResponse == null)
                {
                    this.statusWrapper.SetReady();
                    this.NoContentText = Strings.FailedToRetrievePredictions;
                    return;
                }

                var servicePredictions = result.PredictionResponse;

                for (int i = 0; i < servicePredictions.Request.Items.Count; i++)
                {
                    var request = servicePredictions.Request.Items.ElementAtOrDefault(i);
                    var response = servicePredictions.Predictions.ElementAtOrDefault(i);

                    this.predictions.Add(
                        new PredictionViewModel
                        {
                            Path = request?.Path,
                            ProbableFail = !response?.ProbableSuccess,
                            FailProbability = 1 - response?.SuccessProbability
                        });
                }

                this.statusWrapper.SetByUsingResponse(servicePredictions);
                this.NoContent = false;
            }
            catch (Exception exception)
            {
                this.errorHandler.Handle(exception.Message);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
