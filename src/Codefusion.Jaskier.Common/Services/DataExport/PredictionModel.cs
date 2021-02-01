namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PredictionModel
    {
        [Key]
        public long Id { get; set; }

        public DateTime CreateDateUtc { get; set; }

        [MaxLength(255)]
        public string ProjectName { get; set; }

        public int? CVK { get; set; }

        public double? CVAUC { get; set; }

        public double? AUC { get; set; }

        public double? CMACC { get; set; }

        public int? CMA { get; set; }

        public int? CMB { get; set; }

        public int? CMC { get; set; }

        public int? CMD { get; set; }

        [MaxLength(100)]
        public string Method { get; set; }

        [MaxLength(500)]
        public string Features { get; set; }
    }
}
