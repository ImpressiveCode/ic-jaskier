namespace Codefusion.Jaskier.Common.Services.DataExport
{
    #region Usings

    using System;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class PredictionRequest
    {
        #region Properties
        [Key]
        public long Id { get; set; }

        public bool? BuildResultPredictionClass { get; set; }

        public double? BuildResultFaildPrediction { get; set; }

        [MaxLength(255)]
        public string Guid { get; set; }

        [MaxLength(500)]
        public string Path { get; set; }

        [MaxLength(500)]
        public string OldPath { get; set; }

        public int? TotalNumberOfRevisions { get; set; }

        public int? NumberOfRevisions { get; set; }

        public int? NumberOfDistinctCommitters { get; set; }

        public int? NumberOfModifiedLines { get; set; }

        public int? PreviousBuildResult { get; set; }      

        [MaxLength(255)]
        public string Author { get; set; }

        [MaxLength(255)]
        public string ProjectName { get; set; }

        public long? PredictionModelId { get; set; }

        public int? CCMMax { get; set; }

        public double? CCMAvg { get; set; }

        public double? CCMMd { get; set; }

        public DateTime? CreatedAtUtc { get; set; }
        #endregion
    }
}