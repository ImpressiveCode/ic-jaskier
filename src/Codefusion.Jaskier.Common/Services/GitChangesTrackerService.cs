namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CCMEngine;

    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Helpers;

    using LibGit2Sharp;

    public class GitChangesTrackerService : IChangesTrackerService
    {
        public Task<ChangedFiles> GetChangedFilesAsync(string repositoryPath, IEnumerable<string> filePaths)
        {
            return Task.Run(() => GetChangedFiles(repositoryPath, filePaths));
        }

        private static ChangedFiles GetChangedFiles(string repositoryPath, IEnumerable<string> filePaths)
        {
            try
            {
                using (var repository = new Repository(repositoryPath))
                {
                    string userName = repository.Config.GetValueOrDefault("user.name", string.Empty);

                    var status = repository.RetrieveStatus(
                       new StatusOptions
                       {
                           DetectRenamesInIndex = true,
                           DetectRenamesInWorkDir = true,
                           DisablePathSpecMatch = true,
                           ExcludeSubmodules = true,
                           IncludeUnaltered = true,
                           Show = StatusShowOption.IndexAndWorkDir,
                       });

                    var statusEntries = status.Where(g => g.State.HasFlag(FileStatus.ModifiedInWorkdir) || g.State.HasFlag(FileStatus.RenamedInWorkdir) || g.State.HasFlag(FileStatus.DeletedFromWorkdir)).ToList();

                    var result = new ChangedFiles();

                    if (filePaths != null)
                    {
                        statusEntries = statusEntries
                            .Where(g => filePaths.Any(path => path.IndexOf(g.FilePath, StringComparison.OrdinalIgnoreCase) > -1))
                            .ToList();
                    }

                    if (!statusEntries.Any())
                    {
                        return result;
                    }

                    var changesLookup = repository.Diff.Compare<Patch>(statusEntries.Select(g => g.FilePath), true)
                        .ToDictionary(g => g.Path, g => g);

                    foreach (var loopEntry in statusEntries)
                    {
                        string filePath = loopEntry.FilePath;

                        PatchEntryChanges patchEntryChanges;
                        if (!changesLookup.TryGetValue(filePath, out patchEntryChanges))
                        {
                            continue;
                        }

                        string oldFilePath = filePath;
                        if (loopEntry.IndexToWorkDirRenameDetails != null)
                        {
                            oldFilePath = loopEntry.IndexToWorkDirRenameDetails.OldFilePath;
                        }

                        CyclomaticComplexityMetric metric = null;
                        if (CyclomaticComplexityHelper.IsSupportedFile(filePath))
                        {
                            var fullFilePath = Path.Combine(repositoryPath, filePath);
                            if (File.Exists(fullFilePath))
                            {
                                metric = CyclomaticComplexityHelper.CalculateMetric(
                                    File.OpenRead(fullFilePath),
                                    filePath,
                                    true);
                            }
                        }

                        result.Files.Add(new ChangedFile
                        {
                            Author = userName,
                            Path = filePath,
                            OldPath = oldFilePath,
                            NumberOfModifiedLines = patchEntryChanges.LinesAdded + patchEntryChanges.LinesDeleted,
                            CCMMd = metric?.CCMMd,
                            CCMAvg = metric?.CCMAvg,
                            CCMMax = metric?.CCMMax
                        });
                    }

                    return result;
                }
            }
            catch (RepositoryNotFoundException)
            {
                return null;
            }
        }
    }
}
