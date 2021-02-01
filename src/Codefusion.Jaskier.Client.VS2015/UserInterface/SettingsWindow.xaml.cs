namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Windows.Input;

    public interface ISettingsWindow
    {
        void ShowDialog();
    }

    public partial class SettingsWindow : BaseWindow, ISettingsWindow
    {
        public SettingsWindow(ISettingsView settingsView, IImagesProvider imagesProvider)
        {
            this.InitializeComponent();
            this.Content = settingsView;
            this.Icon = imagesProvider.GetImageSource(ImageType.PluginIcon);
        }     
    }
}
