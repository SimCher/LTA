using System;
using Xamarin.Essentials;

namespace LTA.Mobile.Helpers;

public static class Settings
{
    public const string TopicsPageNavigation = "NavigationPage/TopicsPage";
    public const string MessagesPageNavigation = "NavigationPage/MessagesPage";
    public const string AddTopicNavigation = "NavigationPage/Add";

    public static int UserId
    {
        get => TryGetValue();
        set => Preferences.Set(nameof(UserId), value);
    }

    public static string TopicName
    {
        get => Preferences.Get(nameof(TopicName), string.Empty);
        set => Preferences.Set(nameof(TopicName), value);
    }

    public static PageNames CurrentPage
    {
        get => _currentPage;
        set
        {
            if (value == PageNames.None)
                throw new ArgumentException(nameof(value) + $"in {nameof(CurrentPage)}");

            _currentPage = value;
        }
    }

    public static bool IsCurrentPage(PageNames pageName)
        => CurrentPage.Equals(pageName);

    private static PageNames _currentPage;

    private static int TryGetValue()
    {
        if (int.TryParse(Preferences.Get(nameof(UserId), null), out var returnValue))
        {
            return returnValue;
        }

        throw new InvalidOperationException($"Cannot get value from {nameof(UserId)}");
    }
}