namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Text;

    using Codefusion.Jaskier.Client.VS2015.Commands;
    using Codefusion.Jaskier.Client.VS2015.Services;
    using Codefusion.Jaskier.Common;
    using Codefusion.Jaskier.Common.Services;
    using Codefusion.Jaskier.Common.Services.PredictionsWebClient;

    public interface ISettingsViewModel : IViewModel
    {
        string ServiceUrl { get; set; }

        string ResponseMessage { get; }

        IDelegateCommand TestServiceCommand { get; }

        bool IsBusy { get; }

        object Logo { get; }
    }

    public class SettingsViewModel : NotifyPropertyChangedObject, ISettingsViewModel
    {
        private readonly ISettingsStore settingsStore;        
        private readonly IWebClient webClient;
        private readonly IErrorHandler errorHandler;

        private readonly IDelegateCommand testServiceCommand;

        private string responseMessage;
        private bool isBusy;

        public SettingsViewModel(
            ISettingsStore settingsStore, 
            IWebClient webClient,
            IErrorHandler errorHandler,
            IImagesProvider imagesProvider)
        {
            this.settingsStore = settingsStore;            
            this.webClient = webClient;
            this.errorHandler = errorHandler;

            this.testServiceCommand = new BaseCommand(this.ExecuteTestCommand, this.CanExecuteTestCommand);
            this.Logo = imagesProvider.GetImage(ImageType.PluginIcon);
        }     

        public string ServiceUrl
        {
            get
            {
                return this.settingsStore.ServiceUrl;
            }

            set
            {
                if (this.settingsStore.ServiceUrl == value) return;
                this.settingsStore.ServiceUrl = value;
                this.OnPropertyChanged();
                this.testServiceCommand.InvalidateCanExecute();
            }
        }       

        public string ResponseMessage
        {
            get
            {
                return this.responseMessage;
            }

            set
            {
                if (this.responseMessage != value)
                {
                    this.responseMessage = value;
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

        public object Logo { get; }

        public IDelegateCommand TestServiceCommand => this.testServiceCommand;


        private bool CanExecuteTestCommand(object arg)
        {
            return !this.IsBusy && !string.IsNullOrEmpty(this.ServiceUrl);
        }

        private async void ExecuteTestCommand(object obj)
        {
            try
            {
                this.ResponseMessage = string.Empty;
                this.IsBusy = true;

                var result = await this.webClient.Test();

                var builder = new StringBuilder();
                builder.AppendLine($"Success: {result.IsSuccess}");
                builder.AppendLine($"Status code: {result.StatusCode}");
                builder.AppendLine($"Reason phrase: {result.ReasonPhrase}");
                builder.AppendLine($"Content: {result.Content}");

                this.ResponseMessage = builder.ToString();
            }
            catch (WebServiceException webServiceException)
            {
                this.ResponseMessage = webServiceException.ToString();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void InvalidateCommands()
        {
            this.testServiceCommand.InvalidateCanExecute();
        }
    }
}
