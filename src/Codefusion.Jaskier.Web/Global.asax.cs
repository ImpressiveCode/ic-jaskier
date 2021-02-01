namespace Codefusion.Jaskier.Web
{
    using System.Web.Http;
    using Codefusion.Jaskier.Common;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {            
            DependenciesConfigurator.InitLogger(this.Server.MapPath("~/bin/"));

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(ContainerConfig.Configure);
        }      
    }
}
