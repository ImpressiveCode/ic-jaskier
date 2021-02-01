namespace Codefusion.Jaskier.API
{
    public class ChangedFile
    {
        public string Author { get; set; }

        public string Path { get; set; }

        public string OldPath { get; set; }

        public int NumberOfModifiedLines { get; set; }

        public int? CCMMax { get; set; }

        public double? CCMMd { get; set; }

        public double? CCMAvg { get; set; }
    }
}
