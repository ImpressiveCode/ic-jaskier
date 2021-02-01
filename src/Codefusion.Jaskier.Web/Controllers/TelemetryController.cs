namespace Codefusion.Jaskier.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Web.Services.Telemetry;

    public class TelemetryController : BaseController
    {
        private readonly ITelemetryService telemetryService;

        public TelemetryController(ILogger logger, ITelemetryService telemetryService)
            : base(logger)
        {
            this.telemetryService = telemetryService;
        }

        [HttpPost]
        [Route("PutTelemetry")]
        public async Task<IHttpActionResult> PutTelemetry(PutTelemetryRequest putTelemetryRequest)
        {
            var clientAddress = HttpContext.Current.Request.UserHostAddress;

            putTelemetryRequest.UserIPAddress = clientAddress;

            await this.telemetryService.PutTelemetry(putTelemetryRequest);

            return this.Ok();
        }
    }
}