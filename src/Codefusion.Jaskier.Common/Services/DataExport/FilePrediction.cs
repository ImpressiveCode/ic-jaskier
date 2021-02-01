namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class FilePrediction
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(255)]
        public string FileName { get; set; }

        public bool PredictionEnabled { get; set; }
    }
}