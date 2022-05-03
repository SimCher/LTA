using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace LTA.Mobile.Converters;

public class ByteArrayToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ImageSource returnSource = null;

        if (value != null)
        {
            var memory = new MemoryStream((byte[])value);
            returnSource = ImageSource.FromStream(() => memory);
        }

        return returnSource;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}