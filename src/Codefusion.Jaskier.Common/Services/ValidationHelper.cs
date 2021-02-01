namespace Codefusion.Jaskier.Common.Services
{
    using System;

    public static class ValidationHelper
    {
        public static void IsNotNull(object value, string parameterName)
        {
            if (value == null) throw new ArgumentNullException(parameterName);
        }

        public static void IsNotNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(parameterName);
        }
    }
}