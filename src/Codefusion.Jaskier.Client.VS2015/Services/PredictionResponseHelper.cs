namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Codefusion.Jaskier.Common.Data;

    /// <summary>
    /// Temporary class for viewing prediction responses.
    /// </summary>
    public class PredictionResponseHelper
    {
        public static void ShowInMessageBox(PredictionResponse response)
        {
            if (response == null)
            {
                Show("response == null");
                return;
            }

            var builder = new StringBuilder();

            for (int i = 0; i < response.Request.Items.Count; i++)
            {
                var file = response.Request.Items.ElementAtOrDefault(i);
                var prediction = response.Predictions.ElementAtOrDefault(i);

                if (file != null && prediction != null)
                {
                    builder.AppendLine($"{file.Path} probable success={prediction.ProbableSuccess} success probability={prediction.SuccessProbability}");
                }
            }

            Show(builder.ToString());
        }

        private static void Show(string text)
        {
            MessageBox.Show(Application.Current.MainWindow, text, Strings.Information, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
