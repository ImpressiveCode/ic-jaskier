namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System.Globalization;
    using System.Linq;

    using Codefusion.Jaskier.Client.VS2015.UserInterface;
    using Codefusion.Jaskier.Common.Data;

    public interface IStatusWrapper
    {
        string Text { get; set; }        

        object Image { get; set; }

        void SetWaitingForSolution();

        void SetReady();

        void SetPredictionSuccess(int numberOfPredictions);

        void SetPredictionFail(int numberOfPredictions, int numberOfFails);

        void SetBusy(string busyText);

        void SetWaitingForPredictions();

        void SetByUsingResponse(PredictionResponse predictionResponse);
    }

    public class StatusWrapper : NotifyPropertyChangedObject, IStatusWrapper
    {
        private readonly IImagesProvider imagesProvider;

        private string text;
        private object image;

        public StatusWrapper(IImagesProvider imagesProvider)
        {
            this.imagesProvider = imagesProvider;
        }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public object Image
        {
            get
            {
                return this.image;
            }

            set
            {
                if (this.image != value)
                {
                    this.image = value;
                    this.OnPropertyChanged();
                }
            }
        }        

        public void SetWaitingForSolution()
        {
            this.ChangeState(Strings.StatusTextWaitingForSolution, ImageType.PluginIcon);
        }

        public void SetReady()
        {
            this.ChangeState(Strings.StatusTextReady, ImageType.PluginIcon);
        }

        public void SetPredictionSuccess(int numberOfPredictions)
        {
            var statusText = string.Format(
                CultureInfo.InvariantCulture,
                Strings.DefectsPredictedForXOfXSavedFiles,
                0,
                numberOfPredictions);

            this.ChangeState(statusText, ImageType.SuccessIcon);
        }

        public void SetPredictionFail(int numberOfPredictions, int numberOfFails)
        {
            var statusText = string.Format(
               CultureInfo.InvariantCulture,
               Strings.DefectsPredictedForXOfXSavedFiles,
               numberOfFails,
               numberOfPredictions);

            this.ChangeState(statusText, ImageType.FailIcon);
        }

        public void SetBusy(string busyText)
        {
            this.ChangeState(busyText, ImageType.BusyIcon);
        }

        private void ChangeState(string statusText, ImageType imageType)
        {
            this.Text = $"{Strings.PluginName}: {statusText}";
            this.Image = this.imagesProvider.GetImage(imageType);
        }

        public void SetWaitingForPredictions()
        {
            this.SetBusy(Strings.WaitingForPredictions);
        }

        public void SetByUsingResponse(PredictionResponse predictionResponse)
        {
            if (predictionResponse == null)
            {
                this.SetReady();
                return;
            }

            var numberOfPredictions = predictionResponse.Predictions.Count;
            var numberOfFails = predictionResponse.Predictions.Count(g => g.ProbableSuccess == false);

            if (numberOfFails == 0)
            {
                this.SetPredictionSuccess(numberOfPredictions);
            }
            else
            {
                this.SetPredictionFail(numberOfPredictions, numberOfFails);
            }
        }
    }
}
