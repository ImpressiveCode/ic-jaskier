namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System.ComponentModel.DataAnnotations;

    public class Participant
    {
        [Key]
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string Block { get; set; }

        public string DistinctDeveloper { get; set; }
    }
}