#nullable enable
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Resources;
using Prism.Mvvm;
using Prism.Navigation;

namespace LTA.Mobile.PageModels
{
    /// <summary>
    /// Абстрактный базовый класс PageModel
    /// </summary>
    public abstract class BasePageModel : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        /// <summary>
        /// Сервис реализующий навигацию в приложении
        /// </summary>
        protected INavigationService NavigationService { get; private set; }
        /// <summary>
        /// Сервис реализующий общение с сервером
        /// </summary>
        public IChatService ChatService { get; private set; }

        private string _title;
        private bool _isBusy;
        protected static string _pageMessage;
        protected bool _isPageMessageSeen;

        /// <summary>
        /// Хранит состояние незанятости пользовательского интерфейса
        /// при использовании ReactiveUI
        /// </summary>
        protected IObservable<bool> NotBusyObservable;

        /// <summary>
        /// Заголовок страницы
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Хранит состояние занятости
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        public string PageMessage
        {
            get => _pageMessage;
            protected set => SetProperty(ref _pageMessage, value);
        }

        /// <summary>
        /// Хранит состояние видимости сообщения
        /// </summary>
        public bool IsPageMessageSeen
        {
            get => _isPageMessageSeen;
            protected set => SetProperty(ref _isPageMessageSeen, value);
        }

        public BasePageModel(INavigationService navigationService, IChatService chatService)
        {
            NavigationService = navigationService;
            ChatService = chatService;
            
            ChatService.ConnectionMessage += ChatService_ConnectionMessage;
            // NotBusyObservable = this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy);
        }

        /// <summary>
        /// Совершает попытку асинхронного подключения к серверу
        /// </summary>
        /// <returns></returns>
        protected async Task<string> TryConnectAsync()
        {
            try
            {
                await ChatService.Connect();
                return string.Empty;
            }
            catch (Exception ex)
            {

                return $"{ex.Source}: {ex.Message}";
            }
        }

        /// <summary>
        /// Активирует видимость приложения на время указанное в миллисекундах
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="showTimeMs">Время (мс.)</param>
        protected void ShowMessage(string message, int showTimeMs)
        {
            IsPageMessageSeen = true;
            PageMessage = message;
            Observable.Timer(TimeSpan.FromMilliseconds(showTimeMs)).Subscribe(_ =>
            {
                IsPageMessageSeen = false;
                PageMessage = string.Empty;
            });
        }

        /// <summary>
        /// Активирует видимость приложения на неопределённый срок
        /// </summary>
        /// <param name="message"></param>
        protected void ShowMessage(string message)
        {
            IsPageMessageSeen = true;
            PageMessage = message;
        }
        
        protected virtual void ChatService_ConnectionMessage(object sender, ConnectionMessageEventArgs e)
        {
            Title = e.Message;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}