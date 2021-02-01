namespace Codefusion.Jaskier.Common.Services
{
    #region Usings
    using System;
    using System.Configuration;
    #endregion

    public class AppConfiguration : IAppConfiguration
    {
        #region Private variables
        private readonly ICommandLineParametersParser commandLineParametersParser;

        private readonly Parameter exportDatabaseConnectionString;
        private readonly Parameter importDatabaseConnectionString;
        private readonly Parameter importBuildStatusTableName;
        private readonly Parameter importProjectName;
        private readonly Parameter gitRepositoryPath;
        private readonly Parameter webClientServiceUrl;
        private readonly Parameter buildInfoServiceType;
        private readonly Parameter buildStatisticsInfoServiceType;
        private readonly Parameter changeTrackerServiceType;
        #endregion

        #region  Constructors
        public AppConfiguration(ICommandLineParametersParser commandLineParametersParser)
        {
            this.commandLineParametersParser = commandLineParametersParser;

            this.exportDatabaseConnectionString = this.CreateParameter("AppConfiguration:ExportDatabaseConnectionString");
            this.importDatabaseConnectionString = this.CreateParameter("AppConfiguration:ImportDatabaseConnectionString");
            this.importProjectName = this.CreateParameter("AppConfiguration:ImportProjectName");
            this.importBuildStatusTableName = this.CreateParameter("AppConfiguration:ImportBuildStatusTableName");
            this.gitRepositoryPath = this.CreateParameter("AppConfiguration:GitRepositoryPath");
            this.webClientServiceUrl = this.CreateParameter("AppConfiguration:WebClientServiceUrl");
            this.buildInfoServiceType = this.CreateParameter("AppConfiguration:BuildInfoServiceType", "Codefusion.Jaskier.Common.Services.TravisTorrentBuildInfoService, Codefusion.Jaskier.Common");
            this.buildStatisticsInfoServiceType = this.CreateParameter("AppConfiguration:BuildStatisticsInfoServiceType", "Codefusion.Jaskier.Common.Services.GitBuildStatisticsService, Codefusion.Jaskier.Common");
            this.changeTrackerServiceType = this.CreateParameter("AppConfiguration:ChangeTrackerServiceType", "Codefusion.Jaskier.Common.Services.GitChangesTrackerService, Codefusion.Jaskier.Common");
        }
        #endregion

        #region Properties
        public string ExportDatabaseConnectionString => this.exportDatabaseConnectionString.Value;

        public string ImportDatabaseConnectionString => this.importDatabaseConnectionString.Value;

        public string ImportBuildStatusTableName => this.importBuildStatusTableName.Value;

        public string ImportProjectName => this.importProjectName.Value;

        public string GitRepositoryPath => this.gitRepositoryPath.Value;

        public string WebClientServiceUrl => this.webClientServiceUrl.Value;

        public string BuildInfoServiceType => this.buildInfoServiceType.Value;

        public string BuildStatisticsInfoServiceType => this.buildStatisticsInfoServiceType.Value;

        public string ChangeTrackerServiceType => this.changeTrackerServiceType.Value;
        #endregion

        #region Methods
        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.exportDatabaseConnectionString, this.importDatabaseConnectionString, this.importBuildStatusTableName, this.importProjectName, this.gitRepositoryPath, this.webClientServiceUrl);
        }
        #endregion

        #region My Methods
        private Parameter CreateParameter(string key, string defaultValue = "N/A")
        {
            var fromCommandLine = this.commandLineParametersParser?.ParseParameter(key);
            if (fromCommandLine != null)
            {
                return new Parameter("command line", key, fromCommandLine);
            }

            var fromConfigurationManager = ConfigurationManager.AppSettings[key];
            if (fromConfigurationManager != null)
            {
                return new Parameter(".config", key, fromConfigurationManager);
            }

            return new Parameter("N/A", key, defaultValue);
        }
        #endregion

        #region Nested Types
        private class Parameter
        {
            #region  Constructors
            public Parameter(string source, string name, string value)
            {
                this.Source = source;
                this.Name = name;
                this.Value = value;
            }
            #endregion

            #region Properties
            public string Source { get; }

            public string Name { get; }

            public string Value { get; }
            #endregion

            #region Methods
            public override string ToString()
            {
                return $"({this.Source}) {this.Name} = {this.Value}";
            }
            #endregion
        }
        #endregion
    }
}