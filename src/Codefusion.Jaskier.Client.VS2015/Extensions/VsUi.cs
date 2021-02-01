namespace Codefusion.Jaskier.Client.VS2015.Extensions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    public static class VsUi
    {        
        public static Window MainWindow => Application.Current.MainWindow;

        private static Dispatcher Dispatcher => Application.Current.Dispatcher;

        public static bool AttachTitleBarButton(UIElement button)
        {
            // Visual Studio 2015 - 2017
            var vs20152017Panel = (MainWindow.FindElement("PART_TitleBarFrameControlContainer") as ItemsControl)?.Parent as DockPanel;
            if (vs20152017Panel != null)
            {
                vs20152017Panel.Children.Add(button);
                return true;
            }

            // Visual Studio 2019
            var vs2019Panel = Application.Current.MainWindow.FindElement("WindowTitleBarButtons") as StackPanel;
            if (vs2019Panel != null)
            {
                vs2019Panel.Children.Insert(0, button);
                return true;
            }

            return false;
        }

        public static void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        public static void BeginInvoke(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        public static FrameworkElement FindElement(this Visual visual, string name)
        {
            if (visual == null)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); ++i)
            {
                var child = VisualTreeHelper.GetChild(visual, i) as Visual;

                var frameworkElement = child as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Name == name)
                {
                    return frameworkElement;
                }

                var result = FindElement(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
