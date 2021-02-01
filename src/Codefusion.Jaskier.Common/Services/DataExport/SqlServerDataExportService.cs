namespace Codefusion.Jaskier.Common.Services.DataExport
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CCMEngine;

    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Helpers;

    #endregion

    public class SqlServerDataExportService : IDataExportService
    {
        #region Constants
        private const int ConstStatementsLimit = 200;
        #endregion

        #region Private variables
        private static readonly MyComparer Comparer = new MyComparer();
        private readonly IAppConfiguration appConfiguration;
        #endregion

        #region  Constructors
        public SqlServerDataExportService(IAppConfiguration appConfiguration)
        {
            ValidationHelper.IsNotNull(appConfiguration, nameof(appConfiguration));

            this.appConfiguration = appConfiguration;
        }
        #endregion

        #region Methods
        public async Task<List<string>> GetKnownBuilds()
        {
            using (var context = this.CreateContext())
            {
                var list = await context.Metrics.Where(item => item.ProjectName == this.appConfiguration.ImportProjectName).Select(g => g.BuildCommit).ToListAsync();
                return list.Distinct().ToList();
            }
        }

        public async Task Export(BuildStatistics buildStatistics)
        {
            ValidationHelper.IsNotNull(buildStatistics, nameof(buildStatistics));

            using (var context = this.CreateContext())
            {
                foreach (var loopStat in buildStatistics.CommitStats)
                {
                    foreach (var loopfile in loopStat.FileStats)
                    {
                        context.Metrics.Add(this.CreateStat(buildStatistics.BuildInfo, loopStat, loopfile));
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task RecalculateStatistics(Func<string, Stream> getFileContent)
        {
            await this.RecalculateTotalNumberOfRevisions();
            await this.RecalculatePreviousBuildResult();
            await this.RecalculateCyclomaticComplexity(getFileContent);
        }
        #endregion

        #region My Methods
        private static Metric GetPreviousMetric(Dictionary<string, List<Metric>> metricsDictionary, Metric metric)
        {
            List<Metric> metrics;
            if (!metricsDictionary.TryGetValue(metric.Path, out metrics) || metrics == null || !metrics.Any())
            {
                return null;
            }

            var index = metrics.BinarySearch(metric, Comparer);

            if (index >= 0)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    if (metrics[i].BuildCommitDateTimeLocal < metric.BuildCommitDateTimeLocal)
                    {
                        return metrics[i];
                    }
                }
            }

            int nearest = ~index;

            if (nearest == metrics.Count)
            {
                return metrics.LastOrDefault();
            }

            if (nearest == 0)
            {
                return null;
            }

            for (int i = nearest - 1; i >= 0; i--)
            {
                if (metrics[i].BuildCommitDateTimeLocal < metric.BuildCommitDateTimeLocal)
                {
                    return metrics[i];
                }
            }

            return null;
        }

        private async Task RecalculateCyclomaticComplexity(Func<string, Stream> getFileContent)
        {
            List<Metric> allStats;
            using (var context = this.CreateContext())
            {
                allStats = await this.GetAllStats(context, true, query => query.Where(g => g.CCMAvg == null || g.CCMMax == null || g.CCMMd == null));
            }

            Parallel.For(0, allStats.Count,
                i =>
                    {                       
                        var loopMetric = allStats[i];

                        if (!CyclomaticComplexityHelper.IsSupportedFile(loopMetric.Path))
                        {
                            return;
                        }

                        var stream = getFileContent(loopMetric.ObjectId);
                        if (stream == null)
                        {
                            return;
                        }

                        var cyclomaticCimplexityInfo = CyclomaticComplexityHelper.CalculateMetric(stream, loopMetric.Path, true);
                        if (cyclomaticCimplexityInfo == null)
                        {
                            return;
                        }

                        using (var context = this.CreateContext())
                        {
                            var metric = context.Metrics.FirstOrDefault(g => g.Id == loopMetric.Id);
                            if (metric == null)
                            {
                                return;
                            }

                            metric.CCMMax = cyclomaticCimplexityInfo.CCMMax;
                            metric.CCMAvg = cyclomaticCimplexityInfo.CCMAvg;
                            metric.CCMMd = cyclomaticCimplexityInfo.CCMMd;

                            context.Entry(metric).State = EntityState.Modified;
                            context.SaveChanges();
                        }                      
                    });
        }

        private Metric LatestOf(Metric metric1, Metric metric2)
        {
            if (metric1 == null && metric2 == null)
            {
                return null;
            }

            if (metric1 == null) return metric2;
            if (metric2 == null) return metric1;

            return metric1.BuildCommitDateTimeLocal < metric2.BuildCommitDateTimeLocal ? metric2 : metric1;
        }

        private async Task RecalculateTotalNumberOfRevisions()
        {
            using (var context = this.CreateContext())
            {
                var allStats = await this.GetAllStats(context, true);
                List<string> statements = new List<string>();

                var metricsByPath = allStats.Where(item => item.Path != null).GroupBy(item => item.Path).ToDictionary(item => item.Key, item => item.OrderBy(metric => metric.BuildCommitDateTimeLocal).ToList());
                var metricsByOldPath = allStats.Where(item => item.OldPath != null).GroupBy(item => item.OldPath).ToDictionary(item => item.Key, item => item.OrderBy(metric => metric.BuildCommitDateTimeLocal).ToList());

                for (int i = 0; i < allStats.Count; i++)
                {
                    var current = allStats.ElementAt(i);
                    if (current.TotalNumberOfRevisions != null)
                    {
                        // Do not generate this statistic second time.
                        continue;
                    }

                    ////var previous = allStats.Where(g => g.BuildCommitDateTimeLocal < current.BuildCommitDateTimeLocal && (g.Path == current.Path || g.OldPath == current.Path))
                    ////                        .OrderBy(g => g.BuildCommitDateTimeLocal).LastOrDefault();

                    var metric1 = GetPreviousMetric(metricsByPath, current);
                    var metric2 = GetPreviousMetric(metricsByOldPath, current);

                    var previous = this.LatestOf(metric1, metric2);

                    if (previous == null)
                    {
                        current.TotalNumberOfRevisions = 1;
                    }
                    else
                    {
                        current.TotalNumberOfRevisions = previous.TotalNumberOfRevisions + 1;
                    }

                    statements.Add($"update dbo.Metrics SET TotalNumberOfRevisions={current.TotalNumberOfRevisions} WHERE ID={current.Id};");

                    this.ExecuteStatements(context, statements);
                }

                this.ExecuteStatements(context, statements, true);
            }
        }

        private void ExecuteStatements(DatabaseContext context, List<string> statements, bool flush = false)
        {
            if (statements.Count <= 0) return;

            if (statements.Count > ConstStatementsLimit || flush)
            {
                var query = string.Join(Environment.NewLine, statements);

                context.Database.ExecuteSqlCommand(query);

                statements.Clear();
            }
        }

        private async Task RecalculatePreviousBuildResult()
        {
            using (var context = this.CreateContext())
            {
                var groups = (await this.GetAllStats(context, true)).GroupBy(g => g.BuildCommit).OrderBy(g => g.First().BuildDateTimeLocal).Select(item => new KeyValuePair<string, List<Metric>>(item.Key, item.ToList())).ToList();

                int count = groups.Count;

                List<string> statements = new List<string>();

                for (int i = 1; i < count; i++)
                {
                    this.ExecuteStatements(context, statements);

                    var currentGroup = groups[i];
                    if (currentGroup.Value.All(g => g.PreviousBuildResult != null))
                    {
                        // Do not generate this statistic second time.
                        continue;
                    }

                    var previous = groups[i - 1];

                    var previousResult = previous.Value.First().BuildResult;

                    foreach (var loopStat in currentGroup.Value)
                    {
                        loopStat.PreviousBuildResult = previousResult;

                        statements.Add($"UPDATE dbo.Metrics SET PreviousBuildResult={previousResult} WHERE ID={loopStat.Id};");

                        this.ExecuteStatements(context, statements);
                    }
                }

                this.ExecuteStatements(context, statements, true);
            }
        }

        private async Task<List<Metric>> GetAllStats(DatabaseContext context, bool noTracking = false, Func<IQueryable<Metric>, IQueryable<Metric>> filter = null)
        {
            IQueryable<Metric> query;

            query = noTracking ? context.Metrics.AsNoTracking().AsQueryable() : context.Metrics.AsQueryable();

            if (filter != null)
            {
                query = filter(query);
            }

            string projectName = this.appConfiguration.ImportProjectName;
            if (!string.IsNullOrEmpty(projectName))
            {
                query = query.Where(w => w.ProjectName != null && w.ProjectName == projectName);
            }

            var allStats = await query.OrderBy(g => g.BuildCommitDateTimeLocal).ToListAsync();
            return allStats;
        }

        private DatabaseContext CreateContext()
        {
            return new DatabaseContext(this.appConfiguration.ExportDatabaseConnectionString);
        }

        private Metric CreateStat(BuildInfo current, CommitStats commitStats, FileStats fileStats)
        {
            var stat = new Metric();
            stat.BuildCommit = current.CommitHash;
            stat.BuildResult = (int)current.BuildResult;
            stat.Commit = commitStats.Commit;
            stat.ExportDateUtc = DateTime.UtcNow;
            stat.NumberOfDistinctCommitters = fileStats.NumberOfDistinctCommitters;
            stat.NumberOfModifiedLines = fileStats.NumberOfModifiedLines;
            stat.NumberOfRevisions = fileStats.NumberOfRevisions;
            stat.OldPath = fileStats.OldPath;
            stat.Path = fileStats.Path;
            stat.BuildCommitDateTimeLocal = commitStats.LocalDateTime;
            stat.BuildDateTimeLocal = current.BuildDateTimeLocal;
            stat.Author = commitStats.Author;
            stat.BuildProjectName = current.BuildProjectName;
            stat.ProjectName = this.appConfiguration.ImportProjectName;
            stat.ObjectId = fileStats.GitObjectId;

            return stat;
        }
        #endregion

        #region Nested Types
        public class MyComparer : IComparer<Metric>
        {
            #region Methods
            public int Compare(Metric x, Metric y)
            {
                return x.BuildCommitDateTimeLocal.CompareTo(y.BuildCommitDateTimeLocal);
            }
            #endregion
        }
        #endregion
    }
}