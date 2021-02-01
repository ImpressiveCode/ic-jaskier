namespace Codefusion.Jaskier.Client.CLI
{
    #region Usings
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common;
    using Codefusion.Jaskier.Common.Services;
    using Codefusion.Jaskier.Common.Services.PredictionsWebClient;
    using SimpleInjector;
    using System.Windows.Forms;
    #endregion

    public static class Program
    {
        #region Private variables
        private static readonly FileSystemWatcher FileSystemWatcher = new FileSystemWatcher();
        private static readonly ManualResetEvent ManualResetEvent = new ManualResetEvent(false);
        private static readonly ConcurrentQueue<DateTime> WorkerQueue = new ConcurrentQueue<DateTime>();
        private static bool isClosing;
        #endregion

        #region Methods
        public static int Main(string[] args)
        {
            DependenciesConfigurator.InitLogger();

            try
            {
                var container = Initialize();

                GetPredictions(container).Wait();

                ConfigureWatcher(container);

                // Process the watcher queue.
                Task.Factory.StartNew(async () =>
                {
                    var logger = container.GetInstance<ILogger>();

                    while (true)
                    {
                        if (isClosing) break;

                        ManualResetEvent.WaitOne();

                        try
                        {
                            if (isClosing) break;

                            // Delay to handle bulk operations.
                            Thread.Sleep(TimeSpan.FromSeconds(2));

                            while (!WorkerQueue.IsEmpty)
                            {
                                DateTime item;
                                if (!WorkerQueue.TryDequeue(out item))
                                {
                                    break;
                                }
                            }

                            await GetPredictions(container);
                        }
                        catch (Exception exception)
                        {
                            logger.Error("Could not predict.", exception);
                        }
                        finally
                        {
                            ManualResetEvent.Reset();
                        }
                    }
                });

                Console.ReadKey();
                isClosing = true;
                ManualResetEvent.Set();
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
        #endregion

        #region My Methods
        private static void ConfigureWatcher(Container container)
        {
            var configuration = container.GetInstance<IAppConfiguration>();

            FileSystemWatcher.Path = configuration.GitRepositoryPath;
            FileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.LastWrite;
            FileSystemWatcher.Renamed += FileSystemWatcher_Event;
            FileSystemWatcher.Changed += FileSystemWatcher_Event;
            FileSystemWatcher.Created += FileSystemWatcher_Event;
            FileSystemWatcher.Deleted += FileSystemWatcher_Event;

            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void FileSystemWatcher_Event(object sender, FileSystemEventArgs e)
        {
            WorkerQueue.Enqueue(DateTime.Now);

            ManualResetEvent.Set();
        }

        private static Container Initialize()
        {
            var container = BuildContainer();

            DisplayParameters(container);

            var logger = container.GetInstance<ILogger>();

            logger.Info("Press any key to stop watching.");

            return container;
        }

        private static Container BuildContainer()
        {
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            DependenciesConfigurator.RegisterDependencies(container);
            container.RegisterSingleton<IWebClientSettings, AppConfigurationWebClientSettings>();

            container.Verify(VerificationOption.VerifyAndDiagnose);
            return container;
        }

        private static void DisplayParameters(Container container)
        {
            var configuration = container.GetInstance<IAppConfiguration>();
            var logger = container.GetInstance<ILogger>();

            logger.Info("Using parameters: ");
            logger.Info(configuration.ToString());
        }

        private static async Task GetPredictions(Container container)
        {
            var configuration = container.GetInstance<IAppConfiguration>();
            var service = container.GetInstance<IPredictionService>();
            var logger = container.GetInstance<ILogger>();

            logger.Info("Predictions:");

            var result = await service.GetPredictions(configuration.ImportProjectName, configuration.GitRepositoryPath, null);
            if (result.NumberOfChangedFiles == 0)
            {
                logger.Info("\t(no files changed)");
                return;
            }

            if (result.PredictionResponse == null)
            {
                logger.Info("\t(no response)");
                return;
            }

            var predictions = result.PredictionResponse;            
            if (predictions.Request.Items.Count != predictions.Predictions.Count)
            {
                logger.Error("\t(no predictions - incorrect response)");
            }

            for (int i = 0; i < predictions.Request.Items.Count; i++)
            {
                var item = predictions.Request.Items.ElementAt(i);
                var prediction = predictions.Predictions.ElementAt(i);

                var callback = prediction.ProbableSuccess == true ? (Action<string>)logger.Info : logger.Warn;

                callback($"\t{item.Path}");
                callback($"\t\tModified lines:      {item.NumberOfModifiedLines}");

                if (prediction.ProbableSuccess == true)
                {
                    callback($"\t\tProbable success:    {prediction.ProbableSuccess}");
                }
                else
                {
                    callback($"\t\tProbable success:    {prediction.ProbableSuccess}");
                }

                callback($"\t\tSuccess probability: {prediction.SuccessProbability}");
            }
        }
        #endregion
    }
}