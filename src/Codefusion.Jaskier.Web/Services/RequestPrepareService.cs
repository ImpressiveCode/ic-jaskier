namespace Codefusion.Jaskier.Web.Services
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Common.Services.DataExport;
    using PredictionRequest = Codefusion.Jaskier.Common.Data.PredictionRequest;

    public interface IRequestPrepareService
    {
        Task<PredictionRequest> Prepare(PredictionRequest predictionRequest);
    }

    public class RequestPrepareService : IRequestPrepareService
    {
        private readonly IServiceConfiguration configuration;

        public RequestPrepareService(IServiceConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<PredictionRequest> Prepare(PredictionRequest predictionRequest)
        {
            if (predictionRequest?.Items == null)
            {
                return predictionRequest;
            }

            var paths = predictionRequest.Items.Select(g => g.Path).ToList();
            if (!paths.Any())
            {
                return predictionRequest;
            }

            var oldPaths = predictionRequest.Items.Select(g => g.OldPath).ToList();

            Dictionary<string, Metric> lastStats;
            using (var context = new DatabaseContext(this.configuration.ExportDatabaseConnectionString))
            {
                var query = context.Metrics;

                // Find last statistics for specified paths.
                var subquery = query.Where(g => paths.Contains(g.Path) || oldPaths.Contains(g.Path))                    
                    .GroupBy(g => g.Path)
                    .Select(g => new
                    {
                        Path = g.Key,
                        LastStat = g.OrderByDescending(p => p.BuildCommitDateTimeLocal).Select(p => p).FirstOrDefault()
                    });

                var result = await subquery.ToListAsync();

                lastStats = result.ToDictionary(g => g.Path, g => g.LastStat);
            }

            foreach (var loopItem in predictionRequest.Items)
            {
                Metric stat;
                if (lastStats.TryGetValue(loopItem.Path, out stat))
                {
                    UpdatePrediction(loopItem, stat);                    
                }
                else if (lastStats.TryGetValue(loopItem.OldPath, out stat))
                {
                    UpdatePrediction(loopItem, stat);
                }
            }

            return predictionRequest;
        }

        private static void UpdatePrediction(PredictionRequestFile predictionRequestFile, Metric metric)
        {
            predictionRequestFile.NumberOfDistinctCommitters = 1;
            predictionRequestFile.NumberOfRevisions = 1;
            predictionRequestFile.TotalNumberOfRevisions = (metric.TotalNumberOfRevisions ?? 0) + 1;
            predictionRequestFile.PreviousBuildResult = metric.PreviousBuildResult;
        }
    }
}