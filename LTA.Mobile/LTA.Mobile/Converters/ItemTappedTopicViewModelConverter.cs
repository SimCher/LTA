using System;
using System.Globalization;
using LTA.Mobile.Domain.Models;
using Xamarin.Forms;

namespace LTA.Mobile.Converters;

public class ItemTappedTopicConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Topic topicViewModel)
        {
            throw new ArgumentException("Expected value to be of type Topic");
        }
        return topicViewModel.Id;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}