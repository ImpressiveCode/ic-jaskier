namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using MySql.Data.MySqlClient;

    public class TravisTorrentBuildInfoService : IBuildInfoService
    {
        private readonly IAppConfiguration appConfiguration;

        public TravisTorrentBuildInfoService(IAppConfiguration appConfiguration)
        {
            ValidationHelper.IsNotNull(appConfiguration, nameof(appConfiguration));

            this.appConfiguration = appConfiguration;
        }

        public async Task<IEnumerable<BuildInfo>> GetBuildsInfo(CommitsRange commitsRange)
        {
            string connectionString = this.appConfiguration.ImportDatabaseConnectionString;

            var tableName = this.appConfiguration.ImportBuildStatusTableName;

            var projectName = this.appConfiguration.ImportProjectName;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                StringBuilder sql = new StringBuilder(@"SELECT git_trigger_commit, gh_project_name, tr_status, gh_pushed_at ");
                sql.AppendLine($"FROM {tableName} WHERE gh_project_name = '{projectName}' AND gh_pushed_at IS NOT NULL");

                if (!string.IsNullOrEmpty(commitsRange?.CommitFrom))
                {
                    sql.AppendLine($"AND tr_build_id >= (SELECT tr_build_id FROM {tableName} WHERE git_trigger_commit=:COMMIT_FROM)");
                }

                if (!string.IsNullOrEmpty(commitsRange?.CommitTo))
                {
                    sql.AppendLine($"AND tr_build_id <= (SELECT tr_build_id FROM {tableName} WHERE git_trigger_commit=:COMMIT_TO) ");
                }

                sql.AppendLine("ORDER BY tr_build_id DESC ");

                MySqlCommand cmd = new MySqlCommand(sql.ToString(), conn);

                if (!string.IsNullOrEmpty(commitsRange?.CommitFrom))
                {
                    cmd.Parameters.Add(new MySqlParameter("COMMIT_FROM", MySqlDbType.Text) { Value = commitsRange.CommitFrom });
                }

                if (!string.IsNullOrEmpty(commitsRange?.CommitTo))
                {
                    cmd.Parameters.Add(new MySqlParameter("COMMIT_TO", MySqlDbType.Text) { Value = commitsRange.CommitTo });
                }

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    var infos = new List<BuildInfo>();

                    while (rdr.Read())
                    {
                        infos.Add(ParseBuildInfo(rdr));

                    }
                    return infos;

                }
            }
        }

        private static BuildInfo ParseBuildInfo(IDataRecord reader)
        {
            return new BuildInfo
            {
                CommitHash = GetString(reader, "git_trigger_commit"),
                BuildResult = ParseBuildResult(GetString(reader, "tr_status")),
                BuildDateTimeLocal = GetDateTime(reader, "gh_pushed_at"),
                BuildProjectName = GetString(reader, "gh_project_name")
            };
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

        private static BuildResult ParseBuildResult(string state)
        {
            // Travis statuses:
            //errored
            //failed
            //passed
            //canceled
            //started

            switch (state?.Trim())
            {
                default:
                    return BuildResult.Unknown;
                case "passed":
                    return BuildResult.Success;
                case "failed":
                    return BuildResult.Failed;
            }
        }






    }
}
