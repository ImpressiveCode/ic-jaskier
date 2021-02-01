namespace Codefusion.Jaskier.API
{
    using System.Collections.Generic;

    public class FileStats
    {
        public string Path { get; set; }

        public string OldPath { get; set; }

        public int? NumberOfRevisions { get; set; }

        public int? NumberOfDistinctCommitters { get; set; }

        public int? NumberOfModifiedLines { get; set; }

        public BuildResult BuildResult { get; set; }

        public bool PathHasChanged => this.Path != this.OldPath;

        public string GitObjectId { get; set; }

        public List<CyclomaticComplexityInfo> CyclomaticComplexityInfo { get; } = new List<CyclomaticComplexityInfo>();
    }
}
