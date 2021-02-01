namespace Codefusion.Jaskier.Common.Data
{
    using System.Collections.Generic;

    public class PredictionRequest
    {
        public List<PredictionRequestFile> Items { get; } = new List<PredictionRequestFile>();

        public string ProjectName { get; set; }
    }
}
