namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Oracle.ManagedDataAccess.Client;

    public class BitbucketBuildInfoService : IBuildInfoService
    {
        private readonly IAppConfiguration appConfiguration;

        public BitbucketBuildInfoService(IAppConfiguration appConfiguration)
        {
            ValidationHelper.IsNotNull(appConfiguration, nameof(appConfiguration));

            this.appConfiguration = appConfiguration;
        }

        public async Task<IEnumerable<BuildInfo>> GetBuildsInfo(CommitsRange commitsRange)
        {
            using (var connection = await this.CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    this.ConfigureCommand(command, commitsRange);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var infos = new List<BuildInfo>();

                        while (reader.Read())
                        {
                            infos.Add(ParseBuildInfo(reader));
                        }

                        return infos;
                    }
                }
            }
        }      

        private static BuildInfo ParseBuildInfo(IDataRecord reader)
        {
            return new BuildInfo
            {
                CommitHash = GetString(reader, "CSID"),
                BuildResult = ParseBuildResult(GetString(reader, "STATE")),
                BuildDateTimeLocal = GetDateTime(reader, "DATE_ADDED"),
                BuildProjectName = GetString(reader, "NAME")
            };
        }

        private static BuildResult ParseBuildResult(string state)
        {
            switch (state?.Trim())
            {
                default:
                    return BuildResult.Unknown;
                case "SUCCESSFUL":
                    return BuildResult.Success;
                case "INPROGRESS":
                    return BuildResult.Unknown;
                case "FAILED":
                    return BuildResult.Failed;
            }
        }

        private static string GetString(IDataRecord reader, string fieldName)
        {
            var fieldIndex = reader.GetOrdinal(fieldName);
            return reader.IsDBNull(fieldIndex) ? null : reader.GetString(fieldIndex);
        }

        private static DateTime GetDateTime(IDataRecord reader, string fieldName)
        {
            var fieldIndex = reader.GetOrdinal(fieldName);
            return reader.GetDateTime(fieldIndex);
        }

        private async Task<OracleConnection> CreateConnection()
        {
            var connection = new OracleConnection(this.appConfiguration.ImportDatabaseConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        private void ConfigureCommand(OracleCommand command, CommitsRange commitsRange)
        {
            var tableName = this.appConfiguration.ImportBuildStatusTableName;

            var builder = new StringBuilder();
            builder.Append($@"
                        SELECT 
                            CSID, 
                            NAME, 
                            STATE,
                            DATE_ADDED
                        FROM 
                            {tableName}
                        WHERE 1=1 ");

            if (!string.IsNullOrEmpty(commitsRange?.CommitFrom))
            {
                builder.AppendLine($"AND ID >= (SELECT ID FROM {tableName} WHERE CSID=:COMMIT_FROM)");
                command.Parameters.Add(new OracleParameter("COMMIT_FROM", OracleDbType.Varchar2) { Value = commitsRange.CommitFrom });
            }

            if (!string.IsNullOrEmpty(commitsRange?.CommitTo))
            {
                builder.AppendLine($"AND ID <= (SELECT ID FROM {tableName} WHERE CSID=:COMMIT_TO) ");
                command.Parameters.Add(new OracleParameter("COMMIT_TO", OracleDbType.Varchar2) { Value = commitsRange.CommitTo });
            }

            builder.AppendLine("ORDER BY ID DESC ");

            command.CommandText = builder.ToString();
        }
    }
}