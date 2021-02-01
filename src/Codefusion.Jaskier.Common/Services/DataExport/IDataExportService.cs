namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;

    public interface IDataExportService
    {
        Task<List<string>> GetKnownBuilds();

        Task Export(BuildStatistics buildStatistics);

        Task RecalculateStatistics(Func<string, Stream> getFileContent);
    }
}
