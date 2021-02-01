namespace Codefusion.Jaskier.Export.CLI
{
    using System;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common;
    using Codefusion.Jaskier.Common.Services;
    using SimpleInjector;

    public static class Program
    {
        public static int Main(string[] args)
        {
            DependenciesConfigurator.InitLogger();

            try
            {
                Run().Wait();
            }
            catch (Exception exception)
            {
                Logger.Instance.Error(exception);

                return -1;
            }
            finally
            {
#if DEBUG
                Console.ReadKey();
#endif
            }            

            return 0;
        }

        private static async Task Run()
        {
            var container = new Container();
            DependenciesConfigurator.RegisterDependencies(container);
            container.Verify(VerificationOption.VerifyAndDiagnose);            

            DisplayParameters(container);

            await container.GetInstance<IDataExportJob>().Start();
        }

        private static void DisplayParameters(Container container)
        {
            var configuration = container.GetInstance<IAppConfiguration>();
            var logger = container.GetInstance<ILogger>();

            logger.Info("Using parameters: ");
            logger.Info(configuration.ToString());
        }
    }
}
