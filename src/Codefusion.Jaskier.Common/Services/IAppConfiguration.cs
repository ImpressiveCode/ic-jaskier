namespace Codefusion.Jaskier.Common.Services
{
    public interface IAppConfiguration
    {
        #region Properties
        string ExportDatabaseConnectionString { get; }

        string ImportDatabaseConnectionString { get; }

        string ImportBuildStatusTableName { get; }

        string ImportProjectName { get; }

        string GitRepositoryPath { get; }

        string WebClientServiceUrl { get; }

        string BuildInfoServiceType { get; }

        string BuildStatisticsInfoServiceType { get; }

        string ChangeTrackerServiceType { get; }
        #endregion
    }
}