namespace Codefusion.Jaskier.Common.Data
{
    public class PredictionRequestFile
    {        
        public string Author { get; set; }

        public string Path { get; set; }

        public string OldPath { get; set; }

        public int NumberOfModifiedLines { get; set; }

        /// <summary>
        /// Set by service.
        /// </summary>
        public int NumberOfRevisions { get; set; }

        /// <summary>
        /// Set by service.
        /// </summary>
        public int NumberOfDistinctCommitters { get; set; }        

        /// <summary>
        /// Set by service.
        /// </summary>
        public int TotalNumberOfRevisions { get; set; }

        /// <summary>
        /// Set by service.
        /// </summary>
        public int? PreviousBuildResult { get; set; }

        public int? CCMMax { get; set; }

        public double? CCMMd { get; set; }

        public double? CCMAvg { get; set; }
    }
}