namespace Codefusion.Jaskier.API
{
    using System;
    using System.Collections.Generic;

    public class BuildStatistics
    {
        private readonly List<CommitStats> commitStats = new List<CommitStats>();

        public BuildStatistics(BuildInfo buildInfo)
        {
            this.BuildInfo = buildInfo;
        }

        public BuildInfo BuildInfo { get; }

        public IEnumerable<CommitStats> CommitStats => this.commitStats;        

        public void AddStats(IEnumerable<CommitStats> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            this.commitStats.AddRange(enumerable);
        }

        public void AddStats(CommitStats item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            this.commitStats.Add(item);
        }
    }
}
