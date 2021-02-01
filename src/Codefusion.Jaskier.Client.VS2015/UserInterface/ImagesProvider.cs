namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public interface IImagesProvider
    {
        string GetImagePath(ImageType imageType);

        Image GetImage(ImageType imageType);

        ImageSource GetImageSource(ImageType imageType);
    }

    public class ImagesProvider : IImagesProvider
    {
        public string GetImagePath(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.BusyIcon:
                    return IconsPath("BusyIcon.png");
                case ImageType.SuccessIcon:
                    return IconsPath("SuccessIcon.png");
                case ImageType.FailIcon:
                    return IconsPath("FailIcon.png");
                case ImageType.PluginIcon:
                    return IconsPath("PluginIcon.png");
            }

            return string.Empty;
        }

        public Image GetImage(ImageType imageType)
        {
            return new Image { Source = this.GetImageSource(imageType) };
        }

        public ImageSource GetImageSource(ImageType imageType)
        {
            return new BitmapImage(UriHelper.BuildUri(this.GetImagePath(imageType)));
        }

        private static string IconsPath(string fileName)
        {
            return $"UserInterface/Icons/{fileName}";
        }
    }
}
