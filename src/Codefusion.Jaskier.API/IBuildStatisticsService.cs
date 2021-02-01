namespace Codefusion.Jaskier.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBuildStatisticsService
    {
        Task CreateStatistics(string repositoryPath, IEnumerable<BuildInfo> buildInfos, Func<BuildStatistics, Task> onStatisticsCreated, Predicate<string> shouldSkipCommit);

        string GetFileContent(string repositoryPath, string objectId);
    }
}