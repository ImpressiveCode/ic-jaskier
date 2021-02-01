namespace Codefusion.Jaskier.Common.Services
{
    using Codefusion.Jaskier.Common.Data;

    public class PredictionServiceResult
    {
        public PredictionServiceResult(int numberOfChangedFiles, PredictionResponse predictionResponse)
        {
            this.NumberOfChangedFiles = numberOfChangedFiles;
            this.PredictionResponse = predictionResponse;
        }

        public int NumberOfChangedFiles { get; private set; }

        public PredictionResponse PredictionResponse { get; private set; }
    }
}
