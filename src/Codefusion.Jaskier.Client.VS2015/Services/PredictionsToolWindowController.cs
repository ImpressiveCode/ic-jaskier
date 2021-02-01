namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;

    using Codefusion.Jaskier.Client.VS2015.UserInterface;

    public interface IPredictionsToolWindowController
    {
        void Show();
    }

    public class PredictionsToolWindowController : IPredictionsToolWindowController
    {
        private const string ToolWindowGuid = "9eb94974-6a47-4d08-adc8-b5e1e6fff004";

        private readonly IVsBridge vsBridge;

        private readonly Func<IPredictionsView> predictionsViewFactory;

        private VsFrameController frameController;

        public PredictionsToolWindowController(IVsBridge vsBridge, Func<IPredictionsView> predictionsViewFactory)
        {
            this.vsBridge = vsBridge;
            this.predictionsViewFactory = predictionsViewFactory;
        }

        public void Show()
        {            
            if (this.frameController == null)
            {
                this.frameController = this.vsBridge.CreateSingleInstanceToolWindow(Strings.Predictions, ToolWindowGuid, this.predictionsViewFactory());
            }

            this.frameController.Show();
        }        
    }
}
