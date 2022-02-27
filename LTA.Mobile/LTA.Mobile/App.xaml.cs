using LTA.Mobile.Interfaces;
using LTA.Mobile.PageModels;
using LTA.Mobile.Pages;
using LTA.Mobile.Repositories;
using LTA.Mobile.Services;
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

            await NavigationService.NavigateAsync("NavigationPage/TopicsPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<ITopicRepository, TopicRepository>();
            containerRegistry.RegisterSingleton<IMessageRepository, MessageRepository>();
            containerRegistry.RegisterSingleton<IChatService, ChatService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.Register<IRegisterService, RegisterService>();
            containerRegistry.Register<IPageDialogService, PageDialogService>();

            containerRegistry.RegisterForNavigation<MessagesPage, MessagesPageModel>();
            //containerRegistry.RegisterForNavigation<ChatRoomPage, TestChatRoomPageModel>();
            containerRegistry.RegisterForNavigation<RegistrationPage, RegistrationPageModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageModel>();
            containerRegistry.RegisterForNavigation<TopicsPage, TopicListPageModel>();
        }
    }
}
