namespace Codefusion.Jaskier.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Data;
    using Codefusion.Jaskier.Web.Services;

    public class PredictionController : BaseController
    {
        private readonly IDefectPredictionServiceClient client;
        private readonly IRequestPrepareService requestPrepareService;

        public PredictionController(
            ILogger logger,
            IDefectPredictionServiceClient defectPredictionServiceClient,
            IRequestPrepareService requestPrepareService)
            : base(logger)
        {
            this.client = defectPredictionServiceClient;
            this.requestPrepareService = requestPrepareService;
        }

        [HttpPost]
        [Route("GetPredictions")]
        public async Task<IHttpActionResult> GetPredictions(PredictionRequest predictionRequest)
        {
            return this.Json(await this.client.Predict(await this.requestPrepareService.Prepare(predictionRequest)));
        }        
    }
}