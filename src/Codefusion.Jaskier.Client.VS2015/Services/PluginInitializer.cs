namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Codefusion.Jaskier.Client.VS2015.Extensions;
    using Codefusion.Jaskier.Client.VS2015.UserInterface;

    public interface IPluginInitializer
    {
        Task Initialize(Func<Type, object> serviceProvider);
    }

    public class PluginInitializer : IPluginInitializer
    {
        public async Task Initialize(Func<Type, object> serviceProvider)
        {
            VsBridge.Initialize(serviceProvider);

            await this.AttachTitleBarButtonAsync();

            ServiceLocator.Instance.Resolve<IDocumentSaveWatcher>().Start();
            await ServiceLocator.Instance.Resolve<ISolutionWatcher>().Start();
        }

        private async Task AttachTitleBarButtonAsync()
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null)
                throw new InvalidOperationException("Application.Current.Dispatcher == null");

            while (true)
            {
                var titleBarButton = ServiceLocator.Instance.Resolve<TitleBarButton>();

                var attached = await dispatcher.InvokeAsync(
                                   () => VsUi.AttachTitleBarButton(titleBarButton), 
                                   DispatcherPriority.ApplicationIdle);

                if (attached) break;

                await Task.Delay(1000);
            }
        }
    }
}
