namespace Codefusion.Jaskier.API
{
    using System;
    using System.Collections.Generic;

    public class CommitStats
    {
        private readonly List<FileStats> fileStats = new List<FileStats>();

        public IEnumerable<FileStats> FileStats => this.fileStats;

        public DateTime LocalDateTime { get; set; }

        public string Commit { get; set; }

        public string Author { get; set; }

        public void AddFileStats(IEnumerable<FileStats> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            this.fileStats.AddRange(enumerable);
        }

        public void AddFileStats(FileStats item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.fileStats.Add(item);
        }
    }
}
