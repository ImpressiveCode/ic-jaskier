namespace Codefusion.Jaskier.Common.Helpers
{
    using System;

    public sealed class ExperimentDayCalculator
    {
        public DateTime? PreviousDate { get; set; }

        public int Days { get; set; }

        public int Calculate(DateTime dateTime)
        {
            if (this.PreviousDate == null)
            {
                this.PreviousDate = dateTime;
                return this.Days;
            }

            var timeDiff = dateTime - this.PreviousDate.Value;
            if (timeDiff.TotalDays >= 1)
            {
                this.Days++;
            }

            this.PreviousDate = dateTime;
            return this.Days;
        }
    }
}
