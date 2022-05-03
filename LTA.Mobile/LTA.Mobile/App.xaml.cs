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
using LTA.Mobile.Pages.Messages.Popups;
using LTA.Mobile.Pages.Settings;
using LTA.Mobile.Pages.Test;
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
    /// <summary>
    /// Основной класс приложения
    /// </summary>
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        /// <summary>
        /// Обработчик события запуска приложения
        /// </summary>
        protected override async void OnInitialized()
        {
            InitializeComponent();

            //await NavigationService.NavigateAsync("lta:///NavigationPage/Test");

            if (Settings.UserCode.Equals(string.Empty))
            {
                await NavigationService.NavigateAsync(Settings.LoginPageNavigation);
            }
            else
            {
                await NavigationService.NavigateAsync(Settings.TopicsPageNavigation);
            }
        }

        /// <summary>
        /// Регистрирует все типы в приложении, а такж связывает между собой в рамках контейнера зависимостей
        /// </summary>
        /// <param name="containerRegistry">Контейнер зависимостей</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Регистрация файла приложения
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            //Регистрация контекста базы данных
            containerRegistry.Register<DbContext, LtaClientContext>();
            //Регистрация сервисов
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<IChatService, ChatService>();
            containerRegistry.Register<IRegisterService, RegisterService>();
            //Регистрация репозиториев
            containerRegistry.RegisterSingleton<ITopicRepository, TopicRepository>();
            containerRegistry.RegisterSingleton<IMessageRepository, MessageRepository>();
            //Регистрация навигации
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<BaseTabbedPage, TopicsPage>();
            containerRegistry.RegisterForNavigation<MessagesPage, MessagesPageModel>();
            containerRegistry.RegisterForNavigation<RegistrationPage, RegistrationPageModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageModel>();
            containerRegistry.RegisterForNavigation<TopicsPage, TopicListPageModel>();
            containerRegistry.RegisterForNavigation<Add, AddTopicPageModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageModel>();
            containerRegistry.RegisterForNavigation<Test, TestPageModel>();
            //Регистрация модальных окон
            containerRegistry.Register<IPageDialogService, PageDialogService>();
            containerRegistry.RegisterDialog<ReportPopup, ReportDialogPageModel>("Report");
            containerRegistry.RegisterDialog<SendPicturePopup, SendPicturePopupDialog>("SendPicture");
        }
    }
}
