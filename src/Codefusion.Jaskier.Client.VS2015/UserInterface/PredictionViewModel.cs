namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    using Codefusion.Jaskier.Client.VS2015.Services;

    public class PredictionViewModel : NotifyPropertyChangedObject
    {
        public Color Color
        {
            get
            {
                if (this.FailProbability != null)
                {
                    return ColorService.FadeRedGreenViaYellow((1 - this.FailProbability.Value) * 100, 60.0);
                }

                return Colors.Gray;
            }
        }

        public string FailProbabilityString
        {
            get
            {
                if (this.FailProbability == null)
                {
                    return string.Empty;
                }

                var floor = Math.Floor(this.FailProbability.Value * 100)
                    .ToString(CultureInfo.CurrentUICulture);

                return $"{floor}%";
            }
        }

        public string ProbableSuccessString
        {
            get
            {
                if (this.ProbableFail == true)
                {
                    return Strings.ProbableFail;
                }

                if (this.ProbableFail == false)
                {
                    return Strings.ProbableSuccess;
                }

                return Strings.Unknown;
            }
        }

        public string Path { get; set; }

        public bool? ProbableFail { get; set; }

        public double? FailProbability { get; set; }
    }
}
