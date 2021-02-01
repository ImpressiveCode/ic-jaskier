namespace Codefusion.Jaskier.Web.Services.Configurations
{
    using Codefusion.Jaskier.Common.Data;

    public interface IPredictionServiceConfiguration
    {
        string Serialize(PredictionRequestFile predictionRequestFile);
    }
}