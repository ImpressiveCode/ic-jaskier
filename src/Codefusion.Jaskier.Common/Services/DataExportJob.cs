namespace Codefusion.Jaskier.Common.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Services.DataExport;

    public interface IDataExportJob
    {
        Task Start();
    }

    public class DataExportJob : IDataExportJob
    {
        private readonly ILogger logger;
        private readonly IBuildInfoService buildInfoService;
        private readonly IBuildStatisticsService statisticsService;
        private readonly IDataExportService dataExportService;
        private readonly IAppConfiguration appConfiguration;

        public DataExportJob(
            ILogger logger,
            IBuildInfoService buildInfoService,
            IBuildStatisticsService statisticsService,
            IDataExportService dataExportService, 
            IAppConfiguration appConfiguration)
        {
            ValidationHelper.IsNotNull(logger, nameof(logger));
            ValidationHelper.IsNotNull(buildInfoService, nameof(buildInfoService));
            ValidationHelper.IsNotNull(statisticsService, nameof(statisticsService));
            ValidationHelper.IsNotNull(dataExportService, nameof(dataExportService));

            this.logger = logger;
            this.buildInfoService = buildInfoService;
            this.statisticsService = statisticsService;
            this.dataExportService = dataExportService;
            this.appConfiguration = appConfiguration;
        }

        public async Task Start()
        {
            this.logger.Info("Export job started.");
            this.logger.Info("Retrieving build information...");

            var builds = (await this.buildInfoService.GetBuildsInfo(null)).ToList();
            
            this.logger.Info($"Found {builds.Count} number of builds for commits:");
            this.logger.Info(MyGetAllCommitsString(builds));

            builds.Reverse();

            // Get already known builds. Do not process them again.
            var alreadyExistingBuilds = new HashSet<string>(await this.dataExportService.GetKnownBuilds());

            this.logger.Info("Exporting statistics...");
            await this.statisticsService.CreateStatistics(this.appConfiguration.GitRepositoryPath, builds, this.OnStatisticCreated, g => alreadyExistingBuilds.Contains(g));

            this.logger.Info("Recalculating statistics...");
            await this.dataExportService.RecalculateStatistics(
                objectId =>
                    {
                        var contentString = this.statisticsService.GetFileContent(this.appConfiguration.GitRepositoryPath, objectId);
                        if (contentString == null)
                        {
                            return null;
                        }

                        var memoryStream = new MemoryStream();
                        var writer = new StreamWriter(memoryStream);
                        writer.Write(contentString);
                        writer.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);                        
                        return memoryStream;
                    });

            this.logger.Info("Export job finished.");
        }

        private async Task OnStatisticCreated(BuildStatistics buildStatistics)
        {
            this.LogStatistics(buildStatistics);
            this.logger.Info(string.Empty);
            await this.dataExportService.Export(buildStatistics);            
        }

        private static string MyGetAllCommitsString(List<BuildInfo> allBuilds)
        {
            System.Text.StringBuilder commits = new System.Text.StringBuilder();
            foreach (var build in allBuilds)
            {
                commits.Append(build.CommitHash);
                commits.Append(", ");
            }

            return commits.ToString();
        }

        private void LogStatistics(BuildStatistics buildStatistics)
        {
            foreach (var loopStatistic in buildStatistics.CommitStats)
            {
                this.logger.Info($"    Files changed in commit={loopStatistic.Commit}");

                foreach (var loopStatisticFileStat in loopStatistic.FileStats)
                {
                    this.logger.Info($"        {loopStatisticFileStat.Path}");

                    if (loopStatisticFileStat.PathHasChanged)
                    {
                        this.logger.Info($"        old: {loopStatisticFileStat.OldPath}");
                    }

                    this.logger.Info($"        Number of distinct commiters: {loopStatisticFileStat.NumberOfDistinctCommitters}");
                    this.logger.Info($"        Modified lines: {loopStatisticFileStat.NumberOfModifiedLines}");
                    this.logger.Info($"        Number of revisions: {loopStatisticFileStat.NumberOfRevisions}");                            
                }
            }
        }
    }
}
