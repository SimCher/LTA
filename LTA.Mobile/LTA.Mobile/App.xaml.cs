using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Application.Services;
using LTA.Mobile.Data.Context;
using LTA.Mobile.Data.Repositories;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Helpers;
using LTA.Mobile.PageModels;
using LTA.Mobile.Pages.Base;
using LTA.Mobile.Pages.Identity;
using LTA.Mobile.Pages.Messages;
using LTA.Mobile.Pages.Settings;
using LTA.Mobile.Pages.Topics;
using LTA.Mobile.Pages.Topics.Popups;
using Microsoft.EntityFrameworkCore;
using Prism;
using Prism.Ioc;
using Prism.Services;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace LTA.Mobile
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(Settings.UserCode))
            {
                await NavigationService.NavigateAsync(Settings.LoginPageNavigation);
            }
            else
            {
                await NavigationService.NavigateAsync(Settings.TopicsPageNavigation);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.Register<DbContext, LtaClientContext>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<ITopicRepository, TopicRepository>();
            containerRegistry.RegisterSingleton<IMessageRepository, MessageRepository>();
            containerRegistry.RegisterSingleton<IChatService, ChatService>();

            containerRegistry.Register<IRegisterService, RegisterService>();
            containerRegistry.Register<IPageDialogService, PageDialogService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<BaseTabbedPage, TopicsPage>();
            containerRegistry.RegisterForNavigation<MessagesPage, MessagesPageModel>();
            //containerRegistry.RegisterForNavigation<ChatRoomPage, TestChatRoomPageModel>();
            containerRegistry.RegisterForNavigation<RegistrationPage, RegistrationPageModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageModel>();
            containerRegistry.RegisterForNavigation<TopicsPage, TopicListPageModel>();
            containerRegistry.RegisterForNavigation<Add, AddTopicPageModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageModel>();
            containerRegistry.RegisterDialog<ReportPopup, ReportDialogPageModel>("Report");
        }
    }
}
