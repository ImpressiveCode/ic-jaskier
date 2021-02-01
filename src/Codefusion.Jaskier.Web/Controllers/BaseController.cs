namespace Codefusion.Jaskier.Web.Controllers
{
    using System.Web.Http;
    using Codefusion.Jaskier.API;

    public abstract class BaseController : ApiController
    {
        protected BaseController(ILogger logger)
        {
            this.Logger = logger;
        }

        protected ILogger Logger { get; }      
    }
}