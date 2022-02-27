using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Services.Interfaces;
using LTA.Mobile.Client.Services.Navigation;
using ReactiveUI;

namespace LTA.Mobile.Client.ViewModels
{
    public class TopicViewModel : BaseViewModel
    {
        private INavigationService NavigationService { get; }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private ObservableCollection<Topic> _topics;

        public ObservableCollection<Topic> Topics
        {
            get => _topics;
            set => this.RaiseAndSetIfChanged(ref _topics, value);
        }

        public ICommand TopicSelectedCommand { get; private set; }
        public ICommand FilterOptionChangedCommand { get; private set; }

        public TopicViewModel(IDataStore<User> userDataStore, ITopicDataStore topicDataStore,
            IMessageDataStore messageDataStore, INavigationService navigationService) : base(userDataStore, topicDataStore, messageDataStore)
        {
            NavigationService = navigationService;
            TopicSelectedCommand = ReactiveCommand.CreateFromTask<Topic>(TopicSelected);
            FilterOptionChangedCommand = ReactiveCommand.CreateFromTask<bool>(FilterOptionChanged, NotBusyObservable);
        }

        public override async Task Initialize()
        {
            IsBusy = true;
            await LoadTopicsAsync();
            IsBusy = false;
        }

        public override Task Stop()
        {
            return Task.CompletedTask;
        }

        private async Task LoadTopicsAsync()
        {
            var topics = await TopicDataStore.GetTopicsForUser(AppLocator.CurrentUserId);
            topics = topics.OrderByDescending(t => t.LastMessage.CreationDate);
            Topics = new ObservableCollection<Topic>(topics);
        }

        private Task TopicSelected(Topic topic)
        {
            return NavigationService.GoToPage($"{Constants.MessagePageUrl}?topic_id={topic.Id}");
        }

        private async Task FilterOptionChanged(bool isNotOnline)
        {
            IsBusy = true;
            if (!isNotOnline)
            {
                var topics = await TopicDataStore.GetTopicsForUser(AppLocator.CurrentUserId);
                Topics = new ObservableCollection<Topic>(topics.Where(c => c.Peer.IsOnline));
            }
            else
            {
                await LoadTopicsAsync();
            }

            IsBusy = false;
        }
    }
}