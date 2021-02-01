namespace Codefusion.Jaskier.Web.Services
{
    #region Usings
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Common.Services.DataExport;
    using Codefusion.Jaskier.Web.Services.PredictionServiceAPI;
    using Codefusion.Jaskier.Web.Services.PredictionServiceAPI.Models;
    using PredictionRequest = Codefusion.Jaskier.Common.Data.PredictionRequest;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Newtonsoft.Json.Linq;
    using Codefusion.Jaskier.Web.Services.Configurations;
    using Newtonsoft.Json;
    #endregion

    public class AzureDefectPredictionServiceClient : IDefectPredictionServiceClient
    {
        #region Private variables
        private readonly Random random = new Random();
        private readonly IServiceConfiguration serviceConfiguration;
        private readonly ILogger logger;
        private readonly IRbgService rbgService;
        #endregion

        #region  Constructors
        public AzureDefectPredictionServiceClient(IServiceConfiguration serviceConfiguration, ILogger logger, IRbgService rbgService)
        {
            this.serviceConfiguration = serviceConfiguration;
            this.logger = logger;
            this.rbgService = rbgService;
        }
        #endregion

        #region Methods
        public async Task<PredictionResponse> Predict(PredictionRequest predictionRequest)
        {
            var guid = Guid.NewGuid().ToString();
            var predictionResponseItem = new PredictionResponseItem();
            var response = new PredictionResponse();
            var databaseContext = this.CreateContext();
            var cache = await databaseContext.PredictionRequests.Where(g => g.Guid == guid).ToListAsync();

            await this.InsertRequests(predictionRequest, guid);

            foreach (var loopItem in predictionRequest.Items)
            {
                predictionResponseItem = new PredictionResponseItem();
                await CalculateProbability(loopItem, predictionResponseItem);
                response.Predictions.Add(predictionResponseItem);
            }

            // Experiment
            if (this.serviceConfiguration.ExperimentEnabled)
            {
                await this.InsertFilePredictionForExperiment(predictionRequest, guid);
                response = await this.FilterResponseForExperiment(predictionRequest, response);
            }

            return response;
        }

        private async Task CalculateProbability(PredictionRequestFile predictionRequestFile, PredictionResponseItem predictionResponseItem)
        {
            string apiKey = this.serviceConfiguration.PredictionServiceKey;
            string apiUrl = this.serviceConfiguration.PredictionServiceUrl;
            var configurationTypeFullyQualifiedName = this.serviceConfiguration.PredictionServiceConfigurationType;
            var configurationType = Type.GetType(configurationTypeFullyQualifiedName, true);

            var configuration = (IPredictionServiceConfiguration)Activator.CreateInstance(configurationType);

            predictionResponseItem.SuccessProbability = 0;
            predictionResponseItem.ProbableSuccess = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                HttpResponseMessage httpResponse = await client.PostAsync("", new StringContent(configuration.Serialize(predictionRequestFile)));

                if (httpResponse.IsSuccessStatusCode)
                {
                    string result = await httpResponse.Content.ReadAsStringAsync();

                    JObject jObject = JObject.Parse(result);

                    var scoredProbabilities = jObject.SelectToken("Results.output1[0]['Scored Probabilities']").ToObject<double>();
                    var scoredLabels = jObject.SelectToken("Results.output1[0]['Scored Labels']").ToObject<int>();

                    predictionResponseItem.SuccessProbability = scoredProbabilities;
                    predictionResponseItem.ProbableSuccess = scoredLabels == 1 ? true : false;
                }
            }
        }

        /*public async Task<PredictionResponse> Predict(PredictionRequest predictionRequest)
        {
            if (predictionRequest?.Items == null || !predictionRequest.Items.Any())
            {
                throw new ArgumentException("The request is not correct.", nameof(predictionRequest));
            }

            if (this.serviceConfiguration.PredictionProviderType == PredictionProviderType.ClientExecutable)
            {
                this.ValidatePredictionExecubablePath();
            }

            var guid = Guid.NewGuid().ToString();
            this.logger.Info($"Generating prediction request guid '{guid}'");

            this.logger.Info("Inserting requests");
            await this.InsertRequests(predictionRequest, guid);

            if (this.serviceConfiguration.PredictionProviderType == PredictionProviderType.Server)
            {
                this.logger.Info("Calling prediction service");
                await this.CallPredictionService(guid);
            }
            else
            {
                this.logger.Info("Running prediction executable");
                await this.RunPredictionExecutable(guid);
            }

            this.logger.Info($"Prediction service has finished.");

            this.logger.Info("Generating response");
            return await this.GenerateResponse(predictionRequest, guid);
        }*/
        #endregion

        #region My Methods
        private async Task CallPredictionService(string guid)
        {
            using (PredictionServiceClient client = new PredictionServiceClient(new Uri(this.serviceConfiguration.RServerUrl, UriKind.RelativeOrAbsolute)))
            {
                client.HttpClient.Timeout = this.serviceConfiguration.RServerClientTimeout;

                var loginParameters = new LoginRequest(this.serviceConfiguration.RServerUserName, this.serviceConfiguration.RServerPassword);

                var loginResult = await client.LoginWithHttpMessagesAsync(loginParameters);

                if (string.IsNullOrWhiteSpace(loginResult.Body.AccessToken))
                {
                    throw new SecurityException("Could not log in to prediction service.");
                }

                var headers = client.HttpClient.DefaultRequestHeaders;
                var accessToken = loginResult.Body.AccessToken;
                headers.Remove("Authorization");
                headers.Add("Authorization", $"Bearer {accessToken}");

                var result = await client.PredictWithHttpMessagesAsync(new InputParameters(guid));

                if (!string.IsNullOrWhiteSpace(result.Body.ErrorMessage))
                {
                    throw new Exception(result.Body.ErrorMessage);
                }
            }
        }

        private async Task<PredictionResponse> GenerateResponse(PredictionRequest predictionRequest, string guid)
        {
            using (var databaseContext = this.CreateContext())
            {
                var cache = await databaseContext.PredictionRequests.Where(g => g.Guid == guid).ToListAsync();

                var response = new PredictionResponse();
                foreach (var requestItem in predictionRequest.Items)
                {
                    var cachedItem = cache.FirstOrDefault(g => g.Path == requestItem.Path);
                    if (cachedItem == null)
                    {
                        throw new ApplicationException($"Cached item for path '{requestItem.Path}' not found");
                    }

                    var predictionResponseItem = new PredictionResponseItem();
                    predictionResponseItem.ProbableSuccess = cachedItem.BuildResultPredictionClass;
                    predictionResponseItem.SuccessProbability = 1 - cachedItem.BuildResultFaildPrediction;
                    response.Predictions.Add(predictionResponseItem);
                }

                return response;
            }
        }

        private async Task InsertFilePredictionForExperiment(PredictionRequest predictionRequest, string guid)
        {
            using (var databaseContext = this.CreateContext())
            {
                foreach (var item in predictionRequest.Items)
                {
                    var filePrediction = await databaseContext.FilePredictions
                        .Where(file => file.FileName == item.Path)
                        .SingleOrDefaultAsync();

                    if (filePrediction == null)
                    {
                        var fileCache = databaseContext.FilePredictions.Create();

                        fileCache.FileName = item.Path;
                        fileCache.PredictionEnabled = this.rbgService.GetRandomPredictionEnabledFlag();

                        databaseContext.FilePredictions.Add(fileCache);
                    }
                }

                await databaseContext.SaveChangesAsync();
            }
        }

        private async Task InsertRequests(PredictionRequest predictionRequest, string guid)
        {
            using (var databaseContext = this.CreateContext())
            {
                foreach (var loopItem in predictionRequest.Items)
                {
                    var requestCache = databaseContext.PredictionRequests.Create();

                    requestCache.Guid = guid;

                    requestCache.Author = loopItem.Author;
                    requestCache.Path = loopItem.Path;
                    requestCache.OldPath = loopItem.OldPath;
                    requestCache.NumberOfDistinctCommitters = loopItem.NumberOfDistinctCommitters;
                    requestCache.NumberOfModifiedLines = loopItem.NumberOfModifiedLines;
                    requestCache.NumberOfRevisions = loopItem.NumberOfRevisions;
                    requestCache.TotalNumberOfRevisions = loopItem.TotalNumberOfRevisions;
                    requestCache.PreviousBuildResult = loopItem.PreviousBuildResult;
                    requestCache.ProjectName = predictionRequest.ProjectName;
                    requestCache.CCMMd = loopItem.CCMMd;
                    requestCache.CCMAvg = loopItem.CCMAvg;
                    requestCache.CCMMax = loopItem.CCMMax;
                    requestCache.CreatedAtUtc = DateTime.UtcNow;

                    databaseContext.PredictionRequests.Add(requestCache);
                }

                await databaseContext.SaveChangesAsync();
            }
        }

        private void ValidatePredictionExecubablePath()
        {
            var path = this.serviceConfiguration.PredictionExecutablePath;

            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    throw new InvalidOperationException($"The prediction executable path '{path}' is invalid.");
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Failed to access prediction executable path '{path}'.", exception);
            }
        }

        private Task<int> RunPredictionExecutable(string guid)
        {
            return Task.Factory.StartNew(() =>
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    Arguments = this.serviceConfiguration.PredictionExecutableArgument.Replace("{guid}", guid),
                    FileName = this.serviceConfiguration.PredictionExecutablePath,
                });

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new ApplicationException($"The prediction executable exited with code {process.ExitCode}.");
                }

                return process.ExitCode;
            });
        }

        private DatabaseContext CreateContext()
        {
            return new DatabaseContext(this.serviceConfiguration.ExportDatabaseConnectionString);
        }

        // Experimental setup
        private async Task<PredictionResponse> FilterResponseForExperiment(PredictionRequest request, PredictionResponse response)
        {
            if (!this.serviceConfiguration.ExperimentEnabled)
            {
                return response;
            }

            switch (this.serviceConfiguration.ExperimentMode)
            {
                case ExperimentMode.Participant:
                    return this.FilterResponseForParticipantMode(request, response);

                case ExperimentMode.File:
                    return await this.FilterResponseForFileMode(request, response);
            }

            return response;
        }

        private async Task<PredictionResponse> FilterResponseForFileMode(PredictionRequest request, PredictionResponse response)
        {
            using (var databaseContext = this.CreateContext())
            {
                var index = 0;

                foreach (var item in request.Items)
                {
                    var filePrediction = await databaseContext.FilePredictions
                       .Where(file => file.FileName == item.Path)
                       .SingleOrDefaultAsync();

                    if (filePrediction == null)
                    {
                        return response;
                    }

                    if (filePrediction.PredictionEnabled)
                    {
                        response.Predictions[index].SuccessProbability = null;
                        response.Predictions[index].ProbableSuccess = null;
                    }

                    index++;
                }
            }

            return response;
        }

        private PredictionResponse FilterResponseForParticipantMode(PredictionRequest request, PredictionResponse response)
        {
            var result = this.rbgService.GetNextBlockChar(request, BlockGenerationMode.ImposedParticipant);

            if (result == 'B')
            {
                foreach (var prediction in response.Predictions)
                {
                    prediction.SuccessProbability = null;
                    prediction.ProbableSuccess = null;
                }
            }

            return response;
        }

        #endregion
    }
}