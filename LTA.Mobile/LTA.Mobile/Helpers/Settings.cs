#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace LTA.Mobile.Helpers;

public static class Settings
{
    public const string TopicsPageNavigation = "NavigationPage/TopicsPage";
    public const string MessagesPageNavigation = "NavigationPage/MessagesPage";
    public const string AddTopicNavigation = "NavigationPage/Add";

    public static string UserCode
    {
        get => Preferences.Get(nameof(UserCode), string.Empty);
        set => Preferences.Set(nameof(UserCode), value);
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

    public static void SaveCollection(ICollection<string> collection, string key)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        var json = JsonConvert.SerializeObject(collection);
        Preferences.Set(key, json);
    }

    public static ICollection<string>? GetCollection(string key)
    {
        var collection = Preferences.Get(key, null);

        if (collection == null) return null;

        var collectionObj = JsonConvert.DeserializeObject<ICollection<string>>(collection);

        return collectionObj ?? throw new NullReferenceException("Cannot deserialize the collection");

    }

    private static PageNames _currentPage;
}