using System;
using System.Threading.Tasks;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Services.Interfaces;
using LTA.Mobile.Client.ViewModels.Interfaces;
using ReactiveUI;

namespace LTA.Mobile.Client.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IViewModel
    {
        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        protected IDataStore<User> UserDataStore;
        protected IMessageDataStore MessageDataStore;
        protected ITopicDataStore TopicDataStore;
        protected IObservable<bool> NotBusyObservable;

        public BaseViewModel(IDataStore<User> userDataStore,
            ITopicDataStore topicDataStore, IMessageDataStore messageDataStore)
        {
            TopicDataStore = topicDataStore;
            MessageDataStore = messageDataStore;
            UserDataStore = userDataStore;
            NotBusyObservable = this.WhenAnyValue(vm => vm.IsBusy, _isBusy => !_isBusy);
        }

        public abstract Task Initialize();

        public abstract Task Stop();
    }
}