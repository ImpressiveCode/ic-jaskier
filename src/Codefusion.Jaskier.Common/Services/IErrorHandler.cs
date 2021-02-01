namespace Codefusion.Jaskier.Common.Services
{
    using System;

    public interface IErrorHandler
    {
        void Handle(string message);

        void Handle(string message, Exception exception);
    }
}
