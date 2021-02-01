namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Shell;

    public partial class TitleBarButton : Button, INonClientArea
    {
        public TitleBarButton(ITitleBarButtonViewModel titleBarButtonViewModel)
        {
            this.InitializeComponent();
            this.DataContext = titleBarButtonViewModel;            
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            this.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        /// <summary>
        /// It's required to make the button clickable.
        /// </summary>
        int INonClientArea.HitTest(Point point)
        {
            return 1;
        }
    }
}
