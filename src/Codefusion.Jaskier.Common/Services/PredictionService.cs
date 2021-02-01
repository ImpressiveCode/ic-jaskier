namespace Codefusion.Jaskier.Common.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Common.Services.PredictionsWebClient;

    public interface IPredictionService
    {
        Task<PredictionServiceResult> GetPredictions(string projectName, string path, IEnumerable<string> filePaths);
    }

    public class PredictionService : IPredictionService
    {
        private readonly IWebClient webClient;
        private readonly IChangesTrackerService changesTrackerService;
        private readonly IErrorHandler errorHandler;
        private readonly ILogger logger;

        public PredictionService(
            IWebClient webClient,
            IChangesTrackerService changesTrackerService,
            IErrorHandler errorHandler,
            ILogger logger)
        {
            this.webClient = webClient;
            this.changesTrackerService = changesTrackerService;
            this.errorHandler = errorHandler;
            this.logger = logger;
        }

        public async Task<PredictionServiceResult> GetPredictions(string projectName, string path, IEnumerable<string> filePaths)
        {
            this.logger.Info("Detecting changed files...");

            var changedFiles = await this.DetectChanges(path, filePaths);

            if (changedFiles == null || changedFiles.Files.Count == 0)
            {
                this.logger.Info("No files changed. Nothing to do.");
                return new PredictionServiceResult(0, null);
            }

            foreach (var loopChangedFile in changedFiles.Files)
            {
                this.logger.Info($"\t({loopChangedFile.NumberOfModifiedLines}  lines) {loopChangedFile.Path}");
            }

            this.logger.Info("Calling the service...");
            PredictionRequest request = BuildRequest(changedFiles);
            request.ProjectName = projectName;

            PredictionResponse response;

            try
            {
                response = await this.webClient.GetPredictions(request);
            }
            catch (WebServiceException webServiceException)
            {
                this.errorHandler.Handle(webServiceException.Message, webServiceException);
                return new PredictionServiceResult(changedFiles.Files.Count, null);
            }

            if (response == null)
            {
                response = new PredictionResponse();
            }

            response.Request = request;

            return new PredictionServiceResult(changedFiles.Files.Count, response);
        }

        private static PredictionRequest BuildRequest(ChangedFiles changedFiles)
        {
            var request = new PredictionRequest();
            foreach (var loopFile in changedFiles.Files)
            {
                request.Items.Add(new PredictionRequestFile
                {
                    Author = loopFile.Author,
                    Path = loopFile.Path,
                    OldPath = loopFile.OldPath,
                    NumberOfModifiedLines = loopFile.NumberOfModifiedLines,
                    CCMMd = loopFile.CCMMd,
                    CCMAvg = loopFile.CCMAvg,
                    CCMMax = loopFile.CCMMax
                });
            }

            return request;
        }

        private async Task<ChangedFiles> DetectChanges(string path, IEnumerable<string> filePaths)
        {
            return await this.changesTrackerService.GetChangedFilesAsync(path, filePaths);
        }
    }
}
