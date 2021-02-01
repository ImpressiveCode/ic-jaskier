namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;
    using System.Threading.Tasks;

    public interface ISolutionWatcher
    {
        Task Start();
    }

    public class SolutionWatcher : ISolutionWatcher
    {
        private readonly IVsBridge vsBridge;
        private readonly IStatusWrapper statusWrapper;
        private readonly ICachedPredictionService cachedPredictionService;

        public SolutionWatcher(IVsBridge vsBridge, IStatusWrapper statusWrapper, ICachedPredictionService cachedPredictionService)
        {
            this.vsBridge = vsBridge;
            this.statusWrapper = statusWrapper;
            this.cachedPredictionService = cachedPredictionService;
        }

        public async Task Start()
        {
            this.vsBridge.SolutionOpened += this.OnSolutionOpened;
            this.vsBridge.SolutionClosed += this.OnSolutionClosed;

            if (!string.IsNullOrEmpty(this.vsBridge.SolutionFileName))
            {
                await this.HandleSolutionOpened();
            }
        }        

        private async void OnSolutionOpened(object sender, EventArgs e)
        {
            await this.HandleSolutionOpened();
        }

        private async void OnSolutionClosed(object sender, EventArgs e)
        {
            this.statusWrapper.SetWaitingForSolution();
            await this.cachedPredictionService.Reload();
        }

        private async Task HandleSolutionOpened()
        {
            this.statusWrapper.SetReady();
            await this.cachedPredictionService.Reload();
        }
    }
}
