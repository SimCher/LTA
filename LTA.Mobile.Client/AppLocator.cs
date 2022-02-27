using System.Collections.Generic;
using System.Linq;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Services.Interfaces;
using LTA.Mobile.Client.Services.Navigation;
using LTA.Mobile.Client.ViewModels;
using Splat;

namespace LTA.Mobile.Client
{
    public static class AppLocator
    {
        public static IDataStore<User> UserDataStores => Locator.Current.GetService<IDataStore<User>>();
        public static ITopicDataStore ConversationsDataStore => Locator.Current.GetService<ITopicDataStore>();
        public static IMessageDataStore MessagesDataStore => Locator.Current.GetService<IMessageDataStore>();
        public static SettingsViewModel SettingsViewModel => Locator.Current.GetService<SettingsViewModel>();
        public static TopicViewModel ConversationsViewModel => Locator.Current.GetService<ConversationsViewModel>();
        public static MessageViewModel MessagesViewModel => Locator.Current.GetService<MessagesViewModel>();
        public static INavigationService NavigationService => Locator.Current.GetService<INavigationService>();
        public static string CurrentUserId { get; set; }
        public static User CurrentUser { get; set; }

        public static async void Initialize()
        {
            Locator.CurrentMutable.Register<INavigationService>(() => new SimpleNavigationService());
            Locator.CurrentMutable.RegisterConstant<IDataStore<User>>(new UserDataStores());
            var users = await UserDataStores.GetItemsAsync();
            Locator.CurrentMutable.RegisterConstant<IConversationsDataStore>(new ConversationsDataStore(users.Last(), new List<User>(users)));
            var conversations = await ConversationsDataStore.GetItemsAsync();
            Locator.CurrentMutable.RegisterConstant<IMessageDataStore>(new MessagesDataStore(conversations.First()));
            Locator.CurrentMutable.Register(() => new ConversationsViewModel(UserDataStores,
                ConversationsDataStore, MessagesDataStore, NavigationService));
            Locator.CurrentMutable.Register(() => new MessagesViewModel(UserDataStores,
                ConversationsDataStore, MessagesDataStore));
            Locator.CurrentMutable.Register(() => new SettingsViewModel(UserDataStores,
                ConversationsDataStore, MessagesDataStore));

            CurrentUserId = users.Last().Id;
            CurrentUser = users.Last();
        }
    }
}