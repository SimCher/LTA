using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Application.Services;
using LTA.Mobile.Data.Context;
using LTA.Mobile.Data.Repositories;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.PageModels;
using LTA.Mobile.Pages;
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

            //MainPage = new AppShell();

            await NavigationService.NavigateAsync("NavigationPage/TopicsPage");
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
        }
    }
}
