namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;
    using System.Diagnostics;

    using Codefusion.Jaskier.Common.Data;

    public static class TelemetryHelper
    {
        private static readonly Lazy<string> VisualStudioVersion = new Lazy<string>(
            () =>
                {
                    using (var process = Process.GetCurrentProcess())
                    {
                        var versionInfo = process.MainModule.FileVersionInfo;
                        return versionInfo.FileVersion;
                    }
                });

        private static readonly Lazy<string> PluginVersion = new Lazy<string>(
            () => typeof(TelemetryHelper).Assembly.GetName().Version.ToString());

        public static PutTelemetryRequest CreateRequest(string action, string payload = null)
        {            
            return new PutTelemetryRequest
            {
                Action = action,
                Payload = payload,
                VisualStudioVersion = VisualStudioVersion.Value,
                PluginVersion = PluginVersion.Value,
                UserName = Environment.UserName,        
                UserMachineName = Environment.MachineName                           
            };
        }
    }
}
