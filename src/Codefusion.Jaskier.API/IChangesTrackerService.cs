namespace Codefusion.Jaskier.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChangesTrackerService
    {
        /// <summary>
        /// Gets specified changed files in repository.
        /// </summary>
        Task<ChangedFiles> GetChangedFilesAsync(string repositoryPath, IEnumerable<string> filePaths);
    }
}
