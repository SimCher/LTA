
using LTA.Mobile.Interfaces;
using LTA.Mobile.PageModels;
using LTA.Mobile.Pages;
using LTA.Mobile.Repositories;
using LTA.Mobile.Services;
using Prism;
using Prism.Ioc;
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

            await NavigationService.NavigateAsync("NavigationPage/TopicListPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<ITopicRepository, TopicRepository>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            containerRegistry.Register<IChatService, ChatService>();
            containerRegistry.Register<IRegisterService, RegisterService>();

            containerRegistry.RegisterForNavigation<ChatRoomPage, ChatRoomPageViewModel>();
            containerRegistry.RegisterForNavigation<RegistrationPage, RegistrationPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<TopicListPage, TopicListPageViewModel>();
        }
    }
}
