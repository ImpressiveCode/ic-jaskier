namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using Codefusion.Jaskier.Common.Services;
    using Codefusion.Jaskier.Common.Services.PredictionsWebClient;

    public interface ISettingsStore
    {
        bool PredictSavedFiles { get; set; }

        string ServiceUrl { get; set; }
    }

    public class SettingsStore : ISettingsStore
    {
        private readonly IWindowsRegistryProvider provider;
        private readonly IWebClientSettings webClientSettings;

        public SettingsStore(IWindowsRegistryProvider provider, IWebClientSettings webClientSettings)
        {
            this.provider = provider;
            this.webClientSettings = webClientSettings;
        }    

        public bool PredictSavedFiles
        {
            get
            {
                return this.provider.GetBoolean("PredictSavedFiles", true);
            }

            set
            {
                this.provider.SetBoolean("PredictSavedFiles", value);
            }
        }

        public string ServiceUrl
        {
            get
            {
                return this.webClientSettings.ServiceUrl;
            }

            set
            {
                this.webClientSettings.ServiceUrl = value;
            }
        }
    }
}
