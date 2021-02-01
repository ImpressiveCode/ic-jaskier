namespace Codefusion.Jaskier.Web.Services
{
    #region Usings
    using System;
    using System.Configuration;
    #endregion

    public enum PredictionProviderType
    {
        Server,
        ClientExecutable
    }

    public enum ExperimentMode
    {
        Participant,
        File
    }

    public interface IServiceConfiguration
    {
        #region Properties
        string PredictionServiceUrl { get; }

        string PredictionServiceKey { get; }

        string PredictionServiceConfigurationType { get; }

        string ExportDatabaseConnectionString { get; }

        string PredictionExecutablePath { get; }

        string PredictionExecutableArgument { get; }

        string TrainingExecutableArgument { get; }

        string RServerUrl { get; }

        string RServerUserName { get; }

        string RServerPassword { get; }

        PredictionProviderType PredictionProviderType { get; }

        TimeSpan RServerClientTimeout { get; }

        bool ExperimentEnabled { get;  }

        ExperimentMode ExperimentMode { get; }
        #endregion
    }

    public class ServiceConfiguration : IServiceConfiguration
    {
        #region Properties
        public string PredictionServiceUrl => GetString("PredictionServiceUrl");

        public string PredictionServiceKey => GetString("PredictionServiceKey");

        public string PredictionServiceConfigurationType => GetString("PredictionServiceConfigurationType");

        public string ExportDatabaseConnectionString => GetString("ExportDatabaseConnectionString");

        public string PredictionExecutablePath => GetString("PredictionExecutablePath");

        public string PredictionExecutableArgument => GetString("PredictionExecutableArgument");

        public string RServerUrl => GetString("RServerUrl");

        public string RServerUserName => GetString("RServerUserName");

        public string RServerPassword => GetString("RServerPassword");

        public PredictionProviderType PredictionProviderType => (PredictionProviderType)Enum.Parse(typeof(PredictionProviderType), GetString("RPredictionProviderType"), true);

        public string TrainingExecutableArgument => GetString("TrainingExecutableArgument");

        public TimeSpan RServerClientTimeout => TimeSpan.FromSeconds(int.Parse(GetString("RServerClientTimeoutInSeconds")));

        public bool ExperimentEnabled => GetString("ExperimentEnabled") == "true";

        public ExperimentMode ExperimentMode => ExperimentModeFromString(GetString("ExperimentMode"));
        #endregion

        #region My Methods
        private static string GetString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static ExperimentMode ExperimentModeFromString(string experimentMode)
        {
            if (experimentMode == "file")
            {
                return ExperimentMode.File;
            }

            return ExperimentMode.Participant;
        }
        #endregion

    }
}