namespace Codefusion.Jaskier.Web
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;
    using Codefusion.Jaskier.Common;

    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext?.Exception != null)
            {
                Logger.Instance.Error(actionExecutedContext.Exception);
            }            

            base.OnException(actionExecutedContext);
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }
    }
}