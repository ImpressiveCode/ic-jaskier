namespace Codefusion.Jaskier.Client.VS2015
{
    using System;
    using Codefusion.Jaskier.Client.VS2015.Commands;
    using Codefusion.Jaskier.Client.VS2015.Services;
    using Codefusion.Jaskier.Client.VS2015.UserInterface;
    using Codefusion.Jaskier.Common;
    using Codefusion.Jaskier.Common.Services;
    using SimpleInjector;

    public class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> LazyInstance = new Lazy<ServiceLocator>(() => new ServiceLocator());

        private readonly Lazy<Container> lazyContainer;        

        public ServiceLocator()
        {
            this.lazyContainer = new Lazy<Container>(this.CreateContainer);
        }

        public static ServiceLocator Instance => LazyInstance.Value;

        public T Resolve<T>() where T : class
        {
            return this.lazyContainer.Value.GetInstance<T>();
        }

        private Container CreateContainer()
        {
            var container = new Container();

            container.Options.AllowOverridingRegistrations = true;

            DependenciesConfigurator.RegisterDependencies(container);

            // Overrides.
            container.RegisterSingleton<IErrorHandler, ErrorHandler>();

            // UI services.
            container.RegisterSingleton<IPluginInitializer, PluginInitializer>();

            container.RegisterSingleton<ICachedPredictionService, CachedPredictionService>();

            container.RegisterSingleton<IDocumentSaveWatcher, DocumentSaveWatcher>();
            container.RegisterSingleton<ISolutionWatcher, SolutionWatcher>();
         
            container.RegisterSingleton<IVsBridge, VsBridge>();
            
            container.RegisterSingleton<IImagesProvider, ImagesProvider>();
            container.RegisterSingleton<ISettingsStore, SettingsStore>();
            container.RegisterSingleton<IShowSettingsWindowCommand, ShowSettingsWindowCommand>();
            container.RegisterSingleton<IShowPredictionsWindowCommand, ShowPredictionsWindowCommand>();
            container.RegisterSingleton<IShowPredictionsPaneCommand, ShowPredictionsPaneCommand>();

            container.Register<ISettingsViewModel, SettingsViewModel>();
            container.Register<ISettingsView, SettingsView>();
            container.Register<ISettingsWindow, SettingsWindow>();
            container.RegisterInstance<Func<ISettingsWindow>>(() => container.GetInstance<ISettingsWindow>());

            container.RegisterSingleton<IPredictionsViewModel, PredictionsViewModel>();
            container.Register<IPredictionsView, PredictionsView>();
            container.RegisterInstance<Func<IPredictionsView>>(() => container.GetInstance<IPredictionsView>());
            container.Register<IPredictionsWindow, PredictionsWindow>();
            container.RegisterInstance<Func<IPredictionsWindow>>(() => container.GetInstance<IPredictionsWindow>());

            container.RegisterSingleton<IStatusWrapper, StatusWrapper>();

            container.RegisterSingleton<ITitleBarButtonViewModel, TitleBarButtonViewModel>();
            container.RegisterSingleton<TitleBarButton, TitleBarButton>();

            container.RegisterSingleton<IPredictionsToolWindowController, PredictionsToolWindowController>();

            container.Verify(VerificationOption.VerifyAndDiagnose);

            return container;
        }
    }
}
