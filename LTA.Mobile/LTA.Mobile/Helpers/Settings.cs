using System;
using LTA.Mobile.Domain.Models;
using Xamarin.Essentials;

namespace LTA.Mobile.Helpers;

public static class Settings
{
    public static string AppCenterAndroid = "AC_ANDROID";

    public static string UserId
    {
        get => Preferences.Get(nameof(UserId), null);
        set => Preferences.Set(nameof(UserId), value);
    }

    public static string TopicName
    {
        get => Preferences.Get(nameof(TopicName), string.Empty);
        set => Preferences.Set(nameof(TopicName), value);
    }
}