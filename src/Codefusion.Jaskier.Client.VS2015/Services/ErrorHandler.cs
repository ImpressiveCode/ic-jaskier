namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;
    using System.Windows;
    using Codefusion.Jaskier.Common.Services;

    public class ErrorHandler : IErrorHandler
    {
        public void Handle(string message)
        {
            string text = $"{Strings.ErrorOccurredInPlugin}: {message}";

            Show(text);
        }

        public void Handle(string message, Exception exception)
        {
            string text = $"{Strings.ErrorOccurredInPlugin}: {message} {Environment.NewLine}{Strings.Details}: {exception}";

            Show(text);
        }

        private static void Show(string message)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                message,
                Strings.Error,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
