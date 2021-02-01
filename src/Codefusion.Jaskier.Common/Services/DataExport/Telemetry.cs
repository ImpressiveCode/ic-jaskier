namespace Codefusion.Jaskier.Common.Services.DataExport
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Telemetry
    {
        [Key]
        public virtual int Id { get; set; }

        [MaxLength(255)]
        public virtual string UserName { get; set; }

        [MaxLength(255)]
        public virtual string UserMachineName { get; set; }

        [MaxLength(255)]
        public virtual string UserIPAddress { get; set; }

        public virtual string Action { get; set; }

        public virtual string Payload { get; set; }

        public virtual DateTime DateUtc { get; set; }

        [MaxLength(255)]
        public virtual string VisualStudioVersion { get; set; }

        [MaxLength(255)]
        public virtual string PluginVersion { get; set; }
    }
}
