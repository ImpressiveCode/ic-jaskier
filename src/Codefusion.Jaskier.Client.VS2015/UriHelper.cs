namespace Codefusion.Jaskier.Client.VS2015
{
    using System;

    public static class UriHelper
    {
        private static readonly string AssemblyName = typeof(UriHelper).Assembly.GetName().Name;

        public static Uri BuildUri(string resourcePath)
        {
            string path = $"pack://application:,,,/{AssemblyName};component/{resourcePath}";

            return new Uri(path, UriKind.Absolute);
        }
    }
}
