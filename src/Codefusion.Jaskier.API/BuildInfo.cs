namespace Codefusion.Jaskier.API
{
    using System;

    public class BuildInfo
    {
        public string CommitHash { get; set; }

        public BuildResult BuildResult { get; set; }
        
        public DateTime BuildDateTimeLocal { get; set; }  

        public string BuildProjectName { get; set; }
    }
}
