using System;
using System.Globalization;
using Xamarin.Forms;

namespace LTA.Mobile.Client.Styles.ValueConverters
{
    public class DateHumanizerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;
            return date;
            //return date.Humanize(false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}