#nullable enable
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using Prism.Navigation;
using ReactiveUI;

namespace LTA.Mobile.PageModels
{
    public abstract class BasePageModel : ReactiveObject, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IChatService ChatService { get; private set; }

        private string _title;
        private bool _isBusy;
        protected string _pageMessage;
        protected bool _isPageMessageSeen;

        protected IObservable<bool> NotBusyObservable;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public string PageMessage
        {
            get => _pageMessage;
            protected set => this.RaiseAndSetIfChanged(ref _pageMessage, value);
        }

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

        protected async Task<string> TryConnectAsync()
        {
            try
            {
                await ChatService.Connect();
                return string.Empty;
            }
            catch (Exception ex)
            {
                Observable.Timer(TimeSpan.FromMilliseconds(1500)).Subscribe(async _ => await TryConnectAsync());
                return $"{ex.Source}: {ex.Message}";
            }
        }

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