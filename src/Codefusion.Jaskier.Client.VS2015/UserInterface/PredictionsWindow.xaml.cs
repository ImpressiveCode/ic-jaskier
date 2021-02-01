namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    public interface IPredictionsWindow
    {
        void ShowDialog();
    }

    public partial class PredictionsWindow : BaseWindow, IPredictionsWindow
    {
        public PredictionsWindow(IPredictionsView predictionsView, IImagesProvider imagesProvider)
        {
            this.InitializeComponent();
            this.Content = predictionsView;
            this.Icon = imagesProvider.GetImageSource(ImageType.PluginIcon);
        }

        public new void ShowDialog()
        {
            base.ShowDialog();
        }        
    }
}
