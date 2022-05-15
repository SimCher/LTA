#nullable enable
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using Prism.Navigation;
using ReactiveUI;

namespace LTA.Mobile.PageModels
{
    /// <summary>
    /// Абстрактный базовый класс PageModel
    /// </summary>
    public abstract class BasePageModel : ReactiveObject, IInitialize, INavigationAware, IDestructible
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
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        /// <summary>
        /// Хранит состояние занятости
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        public string PageMessage
        {
            get => _pageMessage;
            protected set => this.RaiseAndSetIfChanged(ref _pageMessage, value);
        }

        /// <summary>
        /// Хранит состояние видимости сообщения
        /// </summary>
        public bool IsPageMessageSeen
        {
            get => _isPageMessageSeen;
            protected set => this.RaiseAndSetIfChanged(ref _isPageMessageSeen, value);
        }

        public BasePageModel(INavigationService navigationService, IChatService chatService)
        {
            NavigationService = navigationService;
            ChatService = chatService;
            NotBusyObservable = this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy);
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
                PageMessage = "Нет подключения к сети";
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