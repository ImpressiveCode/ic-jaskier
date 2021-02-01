namespace Codefusion.Jaskier.Common.Data
{
    public class PutTelemetryRequest
    {
        public string UserName { get; set; }

        public virtual string UserMachineName { get; set; }

        public string UserIPAddress { get; set; }

        public string Action { get; set; }

        public string Payload { get; set; }

        public string VisualStudioVersion { get; set; }

        public string PluginVersion { get; set; }
    }
}
