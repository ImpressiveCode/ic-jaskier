namespace Codefusion.Jaskier.Web
{
    using System.Web.Http;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common;
    using Codefusion.Jaskier.Web.Services;
    using Codefusion.Jaskier.Web.Services.Telemetry;

    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;

    public static class ContainerConfig
    {
        public static void Configure(HttpConfiguration configuration)
        {
            var container = new Container();

            container.Register<ILogger, Logger>();
            container.Register<IServiceConfiguration, ServiceConfiguration>();
            container.Register<IDefectPredictionServiceClient, AzureDefectPredictionServiceClient>();
            container.Register<IRequestPrepareService, RequestPrepareService>();
            container.Register<ITrainingService, TrainingService>();
            container.Register<ITelemetryService, TelemetryService>();
            container.Register<IRbgService, RbgService>();

            container.Verify(VerificationOption.VerifyAndDiagnose);

            configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }       
    }
}