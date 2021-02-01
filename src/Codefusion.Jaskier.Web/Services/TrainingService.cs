namespace Codefusion.Jaskier.Web.Services
{
    #region Usings
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Web.Services.TrainingServiceAPI;
    using Codefusion.Jaskier.Web.Services.TrainingServiceAPI.Models;
    #endregion

    public interface ITrainingService
    {
        #region Methods
        Task Train(string projectName);
        #endregion
    }

    public class TrainingService : ITrainingService
    {
        #region Private variables
        private readonly ILogger logger;
        private readonly IServiceConfiguration serviceConfiguration;
        #endregion

        #region  Constructors
        public TrainingService(ILogger logger, IServiceConfiguration serviceConfiguration)
        {
            this.logger = logger;
            this.serviceConfiguration = serviceConfiguration;
        }
        #endregion

        #region Methods

        public Task Train(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentException("Project name cannot be null or empty.");

            this.logger.Info($"Incoming retraining request for project: {projectName}");

            switch (this.serviceConfiguration.PredictionProviderType)
            {
                case PredictionProviderType.Server:

                    return this.CallServer(projectName);
                case PredictionProviderType.ClientExecutable:
                    this.ValidatePredictionExecubablePath();

                    return this.RunExecutable(projectName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region My Methods
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

        private async Task CallServer(string projectName)
        {
            this.logger.Info($"Running retraining of project '{projectName}' on R Server.");

            using (TrainingServiceClient client = new TrainingServiceClient(new Uri(this.serviceConfiguration.RServerUrl, UriKind.RelativeOrAbsolute)))
            {
                client.HttpClient.Timeout = this.serviceConfiguration.RServerClientTimeout;

                var loginParameters = new LoginRequest(this.serviceConfiguration.RServerUserName, this.serviceConfiguration.RServerPassword);

                var loginResult = await client.LoginWithHttpMessagesAsync(loginParameters);

                if (string.IsNullOrWhiteSpace(loginResult?.Body?.AccessToken))
                {
                    throw new SecurityException("Could not log in to prediction service.");
                }

                var headers = client.HttpClient.DefaultRequestHeaders;
                var accessToken = loginResult.Body.AccessToken;
                headers.Remove("Authorization");
                headers.Add("Authorization", $"Bearer {accessToken}");

                var result = await client.TrainAsync(new InputParameters(projectName));

                var consoleOutput = result?.ConsoleOutput;
                if (!string.IsNullOrWhiteSpace(consoleOutput))
                {
                    this.logger.Info($"Training project '{projectName}'. Console output: {consoleOutput}");
                }

                if (!(result?.Success).GetValueOrDefault())
                {
                    var message = result?.ErrorMessage ?? "R Server returned not a success or null.";
                    this.logger.Error(message);

                    throw new Exception(message);
                }
            }
        }

        private Task RunExecutable(string projectName)
        {
            return Task.Factory.StartNew(() =>
                {
                    var startInfo = new ProcessStartInfo
                    {
                        Arguments = this.serviceConfiguration.TrainingExecutableArgument.Replace("{projectName}", projectName),
                        FileName = this.serviceConfiguration.PredictionExecutablePath,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };

                    var process = Process.Start(startInfo);

                    process.WaitForExit();

                    var output = process.StandardOutput.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        this.logger.Info($"Training project '{projectName}'. Console output: {output}");
                    }

                    var error = process.StandardError.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        this.logger.Error($"Training project '{projectName}'. Error: {error}");
                    }

                    if (process.ExitCode != 0)
                    {
                        throw new ApplicationException($"The training executable exited with code {process.ExitCode}.");
                    }

                    return process.ExitCode;
                });
        }
        #endregion
    }
}