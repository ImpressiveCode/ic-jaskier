namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CCMEngine;

    using Codefusion.Jaskier.API;
    using LibGit2Sharp;

    public class GitBuildStatisticsService : IBuildStatisticsService
    {
        private readonly ILogger logger;

        private Repository repository;
        private Branch branch;

        public GitBuildStatisticsService(ILogger logger)
        {
            ValidationHelper.IsNotNull(logger, nameof(logger));

            this.logger = logger;
        }

        public Task CreateStatistics(string repositoryPath, IEnumerable<BuildInfo> buildInfos, Func<BuildStatistics, Task> onStatisticsCreated, Predicate<string> shouldSkipCommit)
        {
            return this.Create(repositoryPath, buildInfos, onStatisticsCreated, shouldSkipCommit);
        }

        public string GetFileContent(string repositoryPath, string objectId)
        {
            using (var localRepository = new Repository(repositoryPath))
            {
                var blob = localRepository.Lookup(objectId) as Blob;
                if (blob == null)
                {
                    return null;
                }

                return blob.GetContentText();
            }
        }     

        private static Dictionary<string, List<BuildInfo>> CreateBuildsMap(IEnumerable<BuildInfo> buildInfos)
        {
            var result = new Dictionary<string, List<BuildInfo>>();
            foreach (var loopInfo in buildInfos)
            {
                List<BuildInfo> list;
                if (!result.TryGetValue(loopInfo.CommitHash, out list))
                {
                    list = new List<BuildInfo>();
                    result[loopInfo.CommitHash] = list;
                }

                list.Add(loopInfo);
            }

            return result;
        }

        private static List<BuildCommits> CreateBuildCommits(List<Commit> buildCommits, Dictionary<string, List<BuildInfo>> buildsMap)
        {
            var result = new List<BuildCommits>();

            foreach (var loopCommit in buildCommits)
            {
                var queue = new Queue<Commit>();

                // Create a path from current commit to the next build commit.                
                var start = loopCommit.Parents.LastOrDefault();
                if (start != null) queue.Enqueue(start);

                var walkPath = new BuildCommits(loopCommit);
                bool nextBuildCommitFound = false;
                while (queue.Any())
                {
                    var toCompare = queue.Dequeue();
                    walkPath.PreviousCommits.Add(toCompare);
                    if (buildsMap.ContainsKey(toCompare.Sha))
                    {
                        nextBuildCommitFound = true;
                        break;
                    }

                    var next = toCompare.Parents.LastOrDefault();
                    if (next != null)
                    {
                        queue.Enqueue(next);
                    }
                }

                if (nextBuildCommitFound)
                {
                    result.Add(walkPath);
                }
            }

            return result;
        }

        private async Task Create(string repositoryPath, IEnumerable<BuildInfo> buildInfos, Func<BuildStatistics, Task> onStatisticsCreated, Predicate<string> shouldSkipCommit)
        {
            try
            {
                this.repository = new Repository(repositoryPath);

                var branchesMessage = string.Join(Environment.NewLine, this.repository.Branches.Select(item => $"Remote: {item.IsRemote}, Remote name: '{item.RemoteName}', Name: '{item.FriendlyName}'"));
                this.logger.Info("Available branches: " + branchesMessage);

                this.branch = this.repository.Branches.FirstOrDefault(g => g.IsCurrentRepositoryHead);

                this.logger.Info($"Using repository current branch {this.branch}");

                var buildsMap = CreateBuildsMap(buildInfos);

                var commits = this.GetCommitsInTopologicalOrder();
                var commitsWithExistingBuilds = commits.Where(g => buildsMap.ContainsKey(g.Sha)).ToList();
                var buildCommits = CreateBuildCommits(commitsWithExistingBuilds, buildsMap);

                for (int i = 0; i < buildCommits.Count; i++)
                {
                    var loopBuildCommits = buildCommits.ElementAt(i);

                    if (shouldSkipCommit?.Invoke(loopBuildCommits.BuildCommit.Sha) == true)
                    {
                        this.logger.Info($"Skipping commit {loopBuildCommits.BuildCommit.Sha}");
                        continue;
                    }

                    var infos = buildsMap[loopBuildCommits.BuildCommit.Sha];

                    // There may be multiple builds for single build commit...
                    foreach (var buildInfo in infos)
                    {
                        this.logger.Info($"Creating statistics {i} of {buildCommits.Count}");
                        this.logger.Info($"Build commit              : {loopBuildCommits.BuildCommit}");
                        this.logger.Info($"Included commits          : {string.Join(", ", loopBuildCommits.PreviousCommits.Select(g => g.Sha))}");
                        this.logger.Info($"Build result              : {buildInfo.BuildResult}");
                        this.logger.Info($"Build local date and time : {buildInfo.BuildDateTimeLocal}");

                        var statistics = this.CreateCommitsStats(loopBuildCommits, buildInfo);

                        if (onStatisticsCreated != null)
                        {
                            await onStatisticsCreated(statistics);
                        }

                        // ...but use only one.
                        break;
                    }
                }
            }
            finally
            {
                this.repository?.Dispose();
            }
        }

        private BuildStatistics CreateCommitsStats(BuildCommits buildCommits, BuildInfo buildInfo)
        {
            var result = new BuildStatistics(buildInfo);

            var buildCommit = buildCommits.BuildCommit;

            var compareList = new List<Commit>();
            compareList.Add(buildCommit);
            compareList.AddRange(buildCommits.PreviousCommits);

            var tracker = new FilesStatisticsTracker();

            for (int i = 0; i < compareList.Count; i++)
            {
                var commitA = compareList.ElementAtOrDefault(i);
                var commitB = compareList.ElementAtOrDefault(i + 1);

                if (commitA == null || commitB == null)
                    break;

                var commitStats = new CommitStats();
                commitStats.LocalDateTime = commitA.Author.When.LocalDateTime;
                commitStats.Commit = commitA.Sha;
                commitStats.Author = commitA.Author.Name;

                var diff = this.repository.Diff.Compare<Patch>(commitB.Tree, commitA.Tree);

                foreach (var file in diff)
                {
                    tracker.AddAuthor(file.Path, file.OldPath, commitA.Author.Name);
                    tracker.IncreaseRevision(file.Path, file.OldPath);

                    var fileStats = new FileStats();
                    fileStats.Path = file.Path;
                    fileStats.OldPath = file.OldPath;
                    fileStats.NumberOfModifiedLines = file.LinesAdded + file.LinesDeleted;
                    fileStats.BuildResult = buildInfo.BuildResult;
                    fileStats.GitObjectId = file.Oid.Sha;

                    commitStats.AddFileStats(fileStats);
                }

                result.AddStats(commitStats);
            }

            foreach (var loopStat in result.CommitStats)
            {
                foreach (var loopFile in loopStat.FileStats)
                {
                    loopFile.NumberOfDistinctCommitters = tracker.GetDisctinctAuthorsCount(loopFile.Path);
                    loopFile.NumberOfRevisions = tracker.GetRevisionsCount(loopFile.Path);
                }
            }

            return result;
        }

        private List<Commit> GetCommitsInTopologicalOrder()
        {
            return this.repository.Commits
                .QueryBy(new CommitFilter
                {
                    SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time,
                    IncludeReachableFrom = this.branch
                })
                .ToList();
        }

        private class FilesStatisticsTracker
        {
            private readonly Dictionary<string, FileStatistic> map = new Dictionary<string, FileStatistic>();

            public void AddAuthor(string path, string oldPath, string author)
            {
                this.GetStat(path, oldPath).AddAuthor(author);
            }

            public void IncreaseRevision(string path, string oldPath)
            {
                this.GetStat(path, oldPath).NumberOfRevisions++;
            }

            public int GetDisctinctAuthorsCount(string path)
            {
                return this.GetStat(path, null).Authors.Count;
            }

            public int GetRevisionsCount(string path)
            {
                return this.GetStat(path, null).NumberOfRevisions;
            }

            private FileStatistic GetStat(string path, string oldPath)
            {
                FileStatistic result;
                if (this.map.TryGetValue(path, out result))
                {
                    return result;
                }

                if (this.map.TryGetValue(oldPath, out result))
                {
                    this.map[path] = result;
                    return result;
                }

                result = new FileStatistic();
                this.map[path] = result;

                return result;
            }

            private class FileStatistic
            {
                public HashSet<string> Authors { get; } = new HashSet<string>();

                public int NumberOfRevisions { get; set; }

                public void AddAuthor(string author)
                {
                    if (!this.Authors.Contains(author))
                    {
                        this.Authors.Add(author);
                    }
                }
            }
        }

        private class BuildCommits
        {
            public BuildCommits(Commit buildCommit)
            {
                this.BuildCommit = buildCommit;
                this.PreviousCommits = new List<Commit>();
            }

            private Commit Last => this.PreviousCommits.LastOrDefault();

            public Commit BuildCommit { get; }

            public List<Commit> PreviousCommits { get; }

            public override string ToString()
            {
                return $"{Short(this.BuildCommit)} ### {Short(this.Last)}";
            }

            private static string Short(Commit commit)
            {
                if (commit == null)
                {
                    return "NULL";
                }

                return $"{Limit(commit.Sha, 9)} {Limit(commit.MessageShort, 9)}";
            }

            private static string Limit(string value, int count)
            {
                return value?.Length > count ? value.Substring(0, count) : value;
            }
        }
    }
}
