namespace Codefusion.Jaskier.Common.Services.PredictionsWebClient
{
    public class WindowsRegistryWebClientSettings : IWebClientSettings
    {
        private readonly IWindowsRegistryProvider provider;

        public WindowsRegistryWebClientSettings(IWindowsRegistryProvider provider)
        {
            this.provider = provider;
        }

        public string ServiceUrl
        {
            get
            {
                return this.provider.GetString("ServiceUrl", Constants.DefaultAzureServiceUrl);
            }

            set
            {
                this.provider.SetString("ServiceUrl", value);
            }
        }
    }
}
