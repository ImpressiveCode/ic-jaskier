namespace Codefusion.Jaskier.Common.Services.PredictionsWebClient
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.Common.Data;
    using Newtonsoft.Json;

    using System.Net.Http;

    public interface IWebClient
    {
        Task<TestResult> Test();

        Task<PredictionResponse> GetPredictions(PredictionRequest predictionRequest);

        Task PutTelemetry(PutTelemetryRequest putTelemetryRequest);
    }

    public class WebClient : IWebClient
    {
        private readonly IWebClientSettings webClientSettings;

        public WebClient(IWebClientSettings webClientSettings)
        {
            this.webClientSettings = webClientSettings;
        }

        public async Task<PredictionResponse> GetPredictions(PredictionRequest predictionRequest)
        {
            string serviceUrl = Properties.Settings.Default.AzureWebService;

            try
            {
                using (var client = this.CreateClient())
                {
                    if (client.BaseAddress.ToString() != serviceUrl) serviceUrl = client.BaseAddress.ToString();

                    client.BaseAddress = new Uri(serviceUrl);

                    var content = JsonConvert.SerializeObject(predictionRequest);

                    var response = await PostAsync(client, "GetPredictions", content);

                    var responseContent = await response.Content.ReadAsStringAsync();


                    return JsonConvert.DeserializeObject<PredictionResponse>(responseContent);
                }
            }
            catch (Exception exception)
            {
                throw new WebServiceException(exception.Message, exception);
            }
        }

        public async Task PutTelemetry(PutTelemetryRequest putTelemetryRequest)
        {
            try
            {
                using (var client = this.CreateClient())
                {
                    var json = JsonConvert.SerializeObject(putTelemetryRequest);

                    await PostAsync(client, "PutTelemetry", json);
                }
            }
            catch (Exception exception)
            {
                throw new WebServiceException(exception.Message, exception);
            }
        }

        public async Task<TestResult> Test()
        {
            try
            {
                using (var client = this.CreateClient())
                {

                    var response = await client.GetAsync(new Uri("Test", UriKind.Relative));
                    var responseContent = await response.Content.ReadAsStringAsync();

                    return new TestResult
                    {
                        StatusCode = response.StatusCode.ToString(),
                        IsSuccess = response.IsSuccessStatusCode,
                        Content = responseContent,
                        ReasonPhrase = response.ReasonPhrase
                    }; 

                }
            }
            catch (Exception exception)
            {
                throw new WebServiceException(exception.Message, exception);
            }
        }

        private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string relativeUrl, string jsonString)
        {
            return await client.PostAsync(
                        new Uri(relativeUrl, UriKind.Relative),
                        new StringContent(jsonString, Encoding.UTF8, "application/json"));
        }

        private HttpClient CreateClient()
        {
            return new HttpClient { BaseAddress = new Uri(this.webClientSettings.ServiceUrl) };
        }       
    }
}
