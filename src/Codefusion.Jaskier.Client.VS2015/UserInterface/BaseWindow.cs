namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Windows;
    using System.Windows.Input;

    public class BaseWindow : Window
    {
        public BaseWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public new void ShowDialog()
        {
            this.Owner = Application.Current?.MainWindow;
            base.ShowDialog();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            base.OnKeyDown(e);
        }
    }
}
