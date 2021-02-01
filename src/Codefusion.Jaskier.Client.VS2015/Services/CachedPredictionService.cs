namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Codefusion.Jaskier.Common.Services;

    public interface ICachedPredictionService
    {
        PredictionServiceResult GetCurrentPredictions();

        Task Reload();

        event EventHandler PredictionsLoading;

        event EventHandler PredictionsLoaded;
    }

    public class CachedPredictionService : ICachedPredictionService
    {
        private readonly IPredictionService predictionService;
        private readonly IVsBridge vsBridge;
        private readonly IStatusWrapper statusWrapper;
        private readonly IErrorHandler errorHandler;

        private PredictionServiceResult cache;

        public CachedPredictionService(IPredictionService predictionService, IVsBridge vsBridge, IStatusWrapper statusWrapper, IErrorHandler errorHandler)
        {
            this.predictionService = predictionService;
            this.vsBridge = vsBridge;
            this.statusWrapper = statusWrapper;
            this.errorHandler = errorHandler;

            this.PredictionsLoading += this.OnPredictionsLoading;
            this.PredictionsLoaded += this.OnPredictionsLoaded;
        }

        public event EventHandler PredictionsLoading;

        public event EventHandler PredictionsLoaded;

        public PredictionServiceResult GetCurrentPredictions()
        {
            return this.cache;
        }

        public async Task Reload()
        {
            await this.Reload(null);
        }

        public async Task Reload(IList<string> changedFiles)
        {
            try
            {
                this.cache = null;
                this.PredictionsLoading?.Invoke(this, EventArgs.Empty);

                if (string.IsNullOrEmpty(this.vsBridge.SolutionFileName))
                {
                    return;
                }

                var projectName = Path.GetFileNameWithoutExtension(this.vsBridge.SolutionFileName);
                var path = Path.GetDirectoryName(this.vsBridge.SolutionFileName);

                this.cache = await this.predictionService.GetPredictions(projectName, path, changedFiles);
            }
            catch (Exception exception)
            {
                this.errorHandler?.Handle("Failed to reload predictions", exception);
            }
            finally
            {
                this.PredictionsLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnPredictionsLoading(object sender, EventArgs e)
        {
            this.statusWrapper.SetWaitingForPredictions();
        }

        private void OnPredictionsLoaded(object sender, EventArgs e)
        {
            var result = this.GetCurrentPredictions();
            this.statusWrapper.SetByUsingResponse(result?.PredictionResponse);
        }
    }
}
