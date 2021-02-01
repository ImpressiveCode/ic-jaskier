namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System.Windows.Media;

    public class ColorService
    {
        /// <summary>
        /// Fades a color from red to green via yellow.
        /// </summary>
        /// <param name="percent">A value between 0 and 100. 0.00 = red, 100.00 = green</param>
        /// <param name="yellowAtPercent">Specifies the percent on which yellow color should be displayed. Example: 50.00</param>
        /// <returns></returns>
        public static Color FadeRedGreenViaYellow(double percent, double yellowAtPercent)
        {
            byte r = 0;
            byte g = 0;

            const byte Max = byte.MaxValue;

            if (percent < yellowAtPercent)
            {
                r = Max;
                var unit = Max / yellowAtPercent;
                g = (byte)(percent * unit);
            }
            else if (percent >= yellowAtPercent)
            {
                var unit = Max / (100 - yellowAtPercent);
                var decreaseBy = (percent - yellowAtPercent) * unit;
                r = (byte)(Max - decreaseBy);
                g = Max;
            }

            return Color.FromRgb(r, g, 0);
        }
    }
}
