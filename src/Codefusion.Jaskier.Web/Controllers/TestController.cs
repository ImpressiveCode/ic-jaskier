namespace Codefusion.Jaskier.Web.Controllers
{
    using System.Net;
    using System.Web.Http;

    using Codefusion.Jaskier.API;

    public class TestController : BaseController
    {
        public TestController(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Please do not remove this method. It's being used for testing the service from UI.
        /// </summary>
        [HttpGet]
        [Route("Test")]
        public IHttpActionResult Test()
        {
            return this.Content(HttpStatusCode.OK, "This is a message from the server.");
        }
    }
}