namespace Codefusion.Jaskier.Web.Services.Telemetry
{
    using System.Threading.Tasks;

    using Codefusion.Jaskier.Common.Data;

    public interface ITelemetryService
    {
        Task PutTelemetry(PutTelemetryRequest putTelemetryRequest);
    }
}