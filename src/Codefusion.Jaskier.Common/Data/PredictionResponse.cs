namespace Codefusion.Jaskier.Common.Data
{
    using System.Collections.Generic;

    public class PredictionResponse
    {
        public PredictionRequest Request { get; set; }

        public List<PredictionResponseItem> Predictions { get; } = new List<PredictionResponseItem>();
    }
}
