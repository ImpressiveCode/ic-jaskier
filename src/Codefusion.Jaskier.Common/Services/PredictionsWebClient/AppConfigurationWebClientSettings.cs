namespace Codefusion.Jaskier.Common.Services.PredictionsWebClient
{
    public class AppConfigurationWebClientSettings : IWebClientSettings
    {
        private readonly IAppConfiguration appConfiguration;

        public AppConfigurationWebClientSettings(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public string ServiceUrl
        {
            get
            {
                return this.appConfiguration.WebClientServiceUrl;
            }

            set
            {                
            }
        }
    }
}
