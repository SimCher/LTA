using System;
using System.Globalization;
using Xamarin.Forms;

namespace LTA.Mobile.Client.Styles.ValueConverters
{
    public class ImageResourceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return null;

            return ImageSource.FromResource($"FreeChat.Resources.Images.{value.ToString()}",
                typeof(ImageResourceValueConverter).Assembly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}