namespace Codefusion.Jaskier.API
{
    using System;

    public interface ILogger
    {
        void Error(Exception exception);

        void Error(string message);

        void Error(string message, Exception exception);

        void Info(string message);

        void Warn(string message);

        void Warn(string message, Exception exception);
    }
}