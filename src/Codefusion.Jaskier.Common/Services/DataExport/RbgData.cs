namespace Codefusion.Jaskier.Common.Services.DataExport
{
    #region Usings
    using System;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class RbgData
    {
        #region Properties
        [Key]
        public long Id { get; set; }

        [MaxLength(500)]
        public string Developer { get; set; }

        public System.DateTime Date { get; set; }

        [MaxLength(1)]
        public string Mode { get; set; }

        public int ExperimentDay { get; set; }
        #endregion
    }
}
