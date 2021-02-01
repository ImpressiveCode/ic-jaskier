namespace Codefusion.Jaskier.Web.Services.Telemetry
{
    using System;
    using System.Threading.Tasks;

    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Common.Services.DataExport;

    public class TelemetryService : ITelemetryService
    {
        private readonly IServiceConfiguration serviceConfiguration;

        public TelemetryService(IServiceConfiguration serviceConfiguration)
        {
            this.serviceConfiguration = serviceConfiguration;
        }

        public async Task PutTelemetry(PutTelemetryRequest putTelemetryRequest)
        {
            using (var context = this.CreateContext())
            {
                var telemetry = context.Telemetries.Create();
                telemetry.Action = putTelemetryRequest.Action;
                telemetry.DateUtc = DateTime.UtcNow;
                telemetry.Payload = putTelemetryRequest.Payload;
                telemetry.PluginVersion = putTelemetryRequest.PluginVersion;
                telemetry.UserIPAddress = putTelemetryRequest.UserIPAddress;
                telemetry.UserName = putTelemetryRequest.UserName;
                telemetry.UserMachineName = putTelemetryRequest.UserMachineName;
                telemetry.VisualStudioVersion = putTelemetryRequest.VisualStudioVersion;

                context.Telemetries.Add(telemetry);

                await context.SaveChangesAsync();
            }
        }

        private DatabaseContext CreateContext()
        {
            return new DatabaseContext(this.serviceConfiguration.ExportDatabaseConnectionString);
        }
    }
}
