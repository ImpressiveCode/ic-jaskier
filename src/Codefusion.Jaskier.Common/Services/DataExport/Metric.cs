namespace Codefusion.Jaskier.Common.Services.DataExport
{
    #region Usings
    using System;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class Metric
    {
        #region Properties
        [Key]
        public long Id { get; set; }

        [MaxLength(500)]
        public string Path { get; set; }

        [MaxLength(500)]
        public string OldPath { get; set; }

        public int? TotalNumberOfRevisions { get; set; }

        public int? NumberOfRevisions { get; set; }

        public int? NumberOfDistinctCommitters { get; set; }

        public int? NumberOfModifiedLines { get; set; }

        public int BuildResult { get; set; }

        public int? PreviousBuildResult { get; set; }

        [MaxLength(255)]
        public string Commit { get; set; }

        [MaxLength(255)]
        public string BuildCommit { get; set; }

        public DateTime ExportDateUtc { get; set; }

        public DateTime BuildDateTimeLocal { get; set; }

        public DateTime BuildCommitDateTimeLocal { get; set; }

        [MaxLength(255)]
        public string BuildProjectName { get; set; }

        [MaxLength(255)]
        public string Author { get; set; }

        [MaxLength(255)]
        public string ProjectName { get; set; }

        public int? CCMMax { get; set; }

        public double? CCMMd { get; set; }

        public double? CCMAvg { get; set; }

        public string ObjectId { get; set; }
        #endregion
    }
}