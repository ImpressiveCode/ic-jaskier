namespace Codefusion.Jaskier.Web.Controllers
{
    #region Usings
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Web.Services;
    #endregion

    public class TrainingController : BaseController
    {
        #region Private variables
        private readonly ITrainingService trainingService;
        #endregion

        #region  Constructors
        public TrainingController(ILogger logger, ITrainingService trainingService)
                : base(logger)
        {
            this.trainingService = trainingService;
        }
        #endregion

        #region Methods
        [HttpPost]
        [Route("Train")]
        public async Task<IHttpActionResult> Train(string projectName)
        {
            try
            {
                await this.trainingService.Train(projectName);

                return this.Ok();
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception);

                return this.InternalServerError(exception);
            }
        }
        #endregion
    }
}