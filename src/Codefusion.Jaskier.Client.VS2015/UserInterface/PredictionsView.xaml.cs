namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Windows.Controls;
    using System.Windows.Input;

    using Codefusion.Jaskier.Client.VS2015.Services;
    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Common.Services.PredictionsWebClient;

    public interface IPredictionsView
    {        
    }

    public partial class PredictionsView : UserControl, IPredictionsView
    {
        private readonly IWebClient webClient;

        public PredictionsView(IPredictionsViewModel predictionsViewModel, IWebClient webClient)
        {            
            this.InitializeComponent();
            this.DataContext = predictionsViewModel;
            this.webClient = webClient;
            this.Loaded += this.OnLoaded;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                await this.webClient.PutTelemetry(TelemetryHelper.CreateRequest(TelemetryAction.PredictionsViewOpened));
            }
            catch (System.Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }

        private void OnDataGridRowMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // I've got no better idea.
            var row = (DataGridRow)sender;
            var context = this.DataContext as IPredictionsViewModel;

            context?.GoToFileCommand?.Execute(row.Item);
        }
    }
}
