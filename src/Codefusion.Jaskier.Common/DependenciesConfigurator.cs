namespace Codefusion.Jaskier.Common
{
    #region Usings
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Services;
    using Codefusion.Jaskier.Common.Services.DataExport;
    using Codefusion.Jaskier.Common.Services.PredictionsWebClient;
    using SimpleInjector;
    #endregion

    public static class DependenciesConfigurator
    {
        #region Methods
        public static void InitLogger(string configurationPath)
        {
            Logger.Configure(configurationPath);
        }

        public static void InitLogger()
        {
            Logger.Configure(Assembly.GetEntryAssembly());
        }

        public static void RegisterDependencies(Container container)
        {
            IAppConfiguration appConfiguration = new AppConfiguration(new CommandLineParametersParser());

            container.RegisterSingleton<ILogger, Logger>();
            container.RegisterSingleton<IAppConfiguration>(appConfiguration);

            container.RegisterSingleton(typeof(IBuildInfoService), ResolveType<IBuildInfoService>(appConfiguration.BuildInfoServiceType));

            container.RegisterSingleton(typeof(IBuildStatisticsService), ResolveType<IBuildStatisticsService>(appConfiguration.BuildStatisticsInfoServiceType));

            container.RegisterSingleton(typeof(IChangesTrackerService), ResolveType<IChangesTrackerService>(appConfiguration.ChangeTrackerServiceType));

            container.RegisterSingleton<IDataExportService, SqlServerDataExportService>();
            container.RegisterSingleton<IDataExportJob, DataExportJob>();

            container.RegisterSingleton<IWindowsRegistryProvider, WindowsRegistryProvider>();

            container.RegisterSingleton<IWebClientSettings, WindowsRegistryWebClientSettings>();
            container.RegisterSingleton<IWebClient, WebClient>();
            container.RegisterSingleton<IErrorHandler, LoggingOnlyErrorHandler>();
            container.RegisterSingleton<IPredictionService, PredictionService>();
        }
        #endregion

        #region My Methods
        private static Type ResolveType<TType>(string value)
        {
            var message = "Invalid configuration. Unable to resolve service type: " + typeof(TType).Name;

            try
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ConfigurationErrorsException(message);

                var splitted = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).ToArray();
                if (splitted.Length != 2) throw new ConfigurationErrorsException(message);

                string typeName = splitted[0];
                string assemblyName = splitted[1];

                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(item => item.GetName().Name == assemblyName);
                if (assembly == null)
                {
                    var fileName = assemblyName + ".dll";
                    assembly = Assembly.LoadFrom(fileName);
                }

                if (assembly == null) throw new ConfigurationErrorsException(message);

                var type = assembly.GetType(typeName);
                if (type == null) throw new ConfigurationErrorsException(message);

                return type;
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException(message, exception);
            }
        }
        #endregion
    }
}