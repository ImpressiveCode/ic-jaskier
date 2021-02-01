namespace Codefusion.Jaskier.Web.Services
{
    using System.Threading.Tasks;
    using Codefusion.Jaskier.Common.Data;

    public interface IDefectPredictionServiceClient
    {
        Task<PredictionResponse> Predict(PredictionRequest predictionRequest);
    }
}