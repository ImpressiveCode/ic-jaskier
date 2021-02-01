namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using Codefusion.Jaskier.API;

    public class LoggingOnlyErrorHandler : IErrorHandler
    {
        private readonly ILogger logger;

        public LoggingOnlyErrorHandler(ILogger logger)
        {
            this.logger = logger;
        }

        public void Handle(string message)
        {            
            this.logger.Error(message);
        }

        public void Handle(string message, Exception exception)
        {
            this.logger.Error(message, exception);
        }
    }
}
