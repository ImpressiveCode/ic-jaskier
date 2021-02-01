namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Codefusion.Jaskier.Client.VS2015.Commands;

    public class FakePredictionsViewModel : NotifyPropertyChangedObject, IPredictionsViewModel
    {
        private readonly ObservableCollection<PredictionViewModel> predictions = new ObservableCollection<PredictionViewModel>();

        public FakePredictionsViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                this.Add("C:\\Project\\File.txt", null, null);
                this.Add("C:\\Project\\File.txt", null, null);
                this.Add("C:\\Project\\File.txt", false, 0.11);
                this.Add("C:\\Project\\File123.txt", false, 0.21);
                this.Add("C:\\Project\\File.txt", false, 0.31);
                this.Add("C:\\Project\\File.txt", true, 0.59);
                this.Add("C:\\Project\\File.txt", true, 0.69);
                this.Add("C:\\Project\\File.txt", true, 0.823412);
                this.Add("C:\\Project\\File.txt", null, null);
                this.Add("C:\\Project\\File.txt", null, null);
            }            
        }

        public IEnumerable<PredictionViewModel> Predictions => this.predictions;

        public IDelegateCommand GoToFileCommand { get; } = new BaseCommand(o => { });

        public bool NoContent { get; } = false;

        public string NoContentText { get; } = string.Empty;

        public IDelegateCommand RefreshCommand { get; }

        public bool IsBusy => true;

        private void Add(string path, bool? probableFail, double? failProbability)
        {
            this.predictions.Add(new PredictionViewModel
                                     {
                                         Path = path,
                                         ProbableFail = probableFail,
                                         FailProbability = failProbability
                                     });
        }
    }
}
