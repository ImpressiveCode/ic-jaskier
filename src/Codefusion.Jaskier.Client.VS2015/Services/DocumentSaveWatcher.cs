namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System.Timers;
    using Codefusion.Jaskier.Client.VS2015.Extensions;

    public interface IDocumentSaveWatcher
    {
        void Start();
    }

    public class DocumentSaveWatcher : IDocumentSaveWatcher
    {
        private readonly IVsBridge vsBridge;
        private readonly ICachedPredictionService predictionService;
        private readonly ISettingsStore settingsStore;

        private readonly Timer timer;

        public DocumentSaveWatcher(IVsBridge vsBridge, ICachedPredictionService predictionService, ISettingsStore settingsStore)
        {
            this.vsBridge = vsBridge;
            this.predictionService = predictionService;
            this.settingsStore = settingsStore;

            this.timer = CreateTimer(this.OnTimerElapsed);
        }

        public void Start()
        {
            this.vsBridge.DocumentSaved += this.OnDocumentSaved;
        }

        private static Timer CreateTimer(ElapsedEventHandler elapsedEventHandler)
        {
            var timer = new Timer();
            timer.AutoReset = false;
            timer.Interval = 3000;
            timer.Elapsed += elapsedEventHandler;
            return timer;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            VsUi.BeginInvoke(this.GetAndPublishPredictions);
        }

        private async void GetAndPublishPredictions()
        {       
            await this.predictionService.Reload();
        }

        private void ResetTimer()
        {
            this.timer.Stop();
            this.timer.Start();
        }

        private void OnDocumentSaved(object sender, DocumentEventArgs e)
        {
            if (!this.settingsStore.PredictSavedFiles)
            {
                return;
            }

            this.ResetTimer();
        }
    }
}
