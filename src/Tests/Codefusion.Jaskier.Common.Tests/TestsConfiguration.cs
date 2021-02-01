namespace Codefusion.Jaskier.Common.Tests
{
    using Codefusion.Jaskier.Common.Services;

    public class TestsConfiguration : IAppConfiguration
    {
        public TestsConfiguration()
        {
            this.ExportDatabaseConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";
            this.ImportDatabaseConnectionString = "Data Source=source;User Id=user;Password=password;";
            this.ImportBuildStatusTableName = "AO_CFE8FA_BUILD_STATUS";

            this.BuildStatisticsInfoServiceType = this.BuildInfoServiceType = this.ChangeTrackerServiceType = "N/A";
        }

        public string ExportDatabaseConnectionString { get; set; }

        public string ImportDatabaseConnectionString { get; set; }

        public string ImportBuildStatusTableName { get; set; }

        public string GitRepositoryPath { get; set; }

        public string ImportProjectName { get; set; }

        public string WebClientServiceUrl { get; set; }

        public string BuildInfoServiceType { get; }

        public string BuildStatisticsInfoServiceType { get; }

        public string ChangeTrackerServiceType { get; }
    }
}
