namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Windows.Controls;

    public interface ISettingsView
    {        
    }
    
    public partial class SettingsView : UserControl, ISettingsView
    {
        public SettingsView(ISettingsViewModel settingsViewModel)
        {
            this.InitializeComponent();
            this.DataContext = settingsViewModel;
        }
    }
}
