namespace Codefusion.Jaskier.Common
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Services;
    using log4net;
    using log4net.Config;

    public class Logger : ILogger
    {
        private const string ConfigFileName = "log4net.config";
        private static readonly Lazy<ILogger> LazyInstance = new Lazy<ILogger>(() => new Logger(), LazyThreadSafetyMode.ExecutionAndPublication);
        private static ILog log;

        public static ILogger Instance => LazyInstance.Value;

        internal static void Configure(Assembly assembly)
        {
            ValidationHelper.IsNotNull(assembly, nameof(assembly));

            Configure(Path.GetDirectoryName(assembly.Location));
        }

        internal static void Configure(string path)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(path, ConfigFileName)));
            log = LogManager.GetLogger(string.Empty);
        }

        public void Error(string message) => log?.Error(message);

        public void Error(Exception exception) => log?.Error(exception);

        public void Error(string message, Exception exception) => log?.Error(message, exception);

        public void Info(string message) => log?.Info(message);

        public void Warn(string message) => log?.Warn(message);

        public void Warn(string message, Exception exception) => log?.Warn(message, exception);
    }
}