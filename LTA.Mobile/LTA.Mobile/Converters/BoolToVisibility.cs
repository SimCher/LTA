using System;
using System.Globalization;
using Xamarin.Forms;

namespace LTA.Mobile.Converters
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringParameter = parameter as string;

            if (stringParameter == "Inverse")
            {
                return !(bool) value;
            }

            return (bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}