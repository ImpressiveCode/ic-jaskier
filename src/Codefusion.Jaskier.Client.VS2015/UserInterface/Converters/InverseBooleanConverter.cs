namespace Codefusion.Jaskier.Client.VS2015.UserInterface.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal sealed class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
