using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Helpers;
using Prism.Navigation;
using Prism.Services;
using MvvmHelpers;
using Prism;
using Prism.Commands;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

public class TopicListPageModel : BasePageModel
{
    private readonly ITopicRepository _topicRepository;
    private ObservableRangeCollection<Topic> _topicList;
    private Topic _topic;
    private bool _isRefreshing;

    public TopicListPageModel(INavigationService navigationService, IChatService chatService,
        ITopicRepository topicRepository, IPageDialogService pageDialogService) : base(navigationService, chatService)
    {
        IsBusy = true;

        _topicRepository = topicRepository;

        //ItemTappedCommand = new DelegateCommand(OnItemTappedAsync);
        NavigateToAddCommand = ReactiveCommand.CreateFromTask(NavigateToAdd);
        ItemTappedCommand = ReactiveCommand.CreateFromTask<Topic>(OnItemTappedAsync);
        FilterOptionChangedCommand = ReactiveCommand.CreateFromTask<string>(FilterOptionChanged, NotBusyObservable);
        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshTopics);
        OnSearchTextChangedCommand = new DelegateCommand(OnSearchTextChanged);
        FilterChangedCommand = new DelegateCommand<string>(FilterChanged);
        SortCommand = new DelegateCommand(ExecuteSortAsync);
        SetIsFavoriteCommand = new DelegateCommand<string>(SetIsFavorite);
        DialogService = pageDialogService;
        IsBusy = false;
    }

    public IPageDialogService DialogService;

    private bool IsNavigate { get; set; }
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => this.RaiseAndSetIfChanged(ref _isRefreshing, value);
    }

    public Topic Topic
    {
        get => _topic;
        set => this.RaiseAndSetIfChanged(ref _topic, value);
    }

    public ObservableRangeCollection<Topic> TopicList
    {
        get => _topicList;
        set => this.RaiseAndSetIfChanged(ref _topicList, value);
    }

    public string SearchText { get; set; }
    public string Filter { get; set; }

    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFilter, value);
            ChangeFilter(value);
        }
    }

    private string _selectedFilter;

    public ICommand ItemTappedCommand { get; set; }
    public ICommand NavigateToAddCommand { get; private set; }
    public ICommand FilterOptionChangedCommand { get; private set; }
    public ICommand RefreshCommand { get; private set; }
    public ICommand OnSearchTextChangedCommand { get; private set; }
    public ICommand FilterChangedCommand { get; private set; }
    public ICommand SetIsFavoriteCommand { get; }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            Settings.CurrentPage = PageNames.Topics;
            PageMessage = await TryConnectAsync();
            ChatService.AddUserInTopic(AddUser);
            ChatService.RemoveUserFromTopic(RemoveUser);

            await RefreshTopics();

            LoadFavorites();
        }
        catch (System.Exception ex)
        {
            await DialogService.DisplayAlertAsync($"Error!", $"{ex.Source}: {ex.Message}", "Got it!");
        }

        finally
        {
            IsBusy = false;
        }
    }

    private void ChangeFilter(string filter)
    {
        switch (filter)
        {
            case "All":
                TopicList = new ObservableRangeCollection<Topic>(_topicRepository.Topics);
                break;
            case "Favorites":
                TopicList = new ObservableRangeCollection<Topic>(TopicList.Where(t => t.IsFavorite));
                break;
            case "My":
                return;
        }
    }

    private async Task RefreshTopics()
    {
        IsRefreshing = true;
        TopicList = new ObservableRangeCollection<Topic>(await _topicRepository.GetAllAsync());
        IsRefreshing = false;
    }

    private void GetTopics(Topic topic)
    {
        AddTopic(topic);
    }

    private async void UpdateTopic(int topicId, Dictionary<string, Color> users, DateTime lastEntry)
    {
        foreach (var (key, value) in users)
        {
            await _topicRepository.AddUserInTopic(new User
            {
                Code = key,
                Color = value
            }, topicId);
        }

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.Topics);
    }

    private async Task AddUser(int topicId, Dictionary<string, Color> users, DateTime lastEntry)
    {
        foreach (var (key, value) in users)
        {
            await _topicRepository.AddUserInTopic(new User
            {
                Code = key,
                Color = value
            }, topicId);
        }

        TopicList = new ObservableRangeCollection<Topic>(await _topicRepository.GetAllAsync());
    }

    private async void RemoveUser(int topicId, string userCode)
    {
        await _topicRepository.RemoveUserFromTopic(userCode, topicId);

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.Topics);
    }

    private void OnSearchTextChanged()
    {
        TopicList = string.IsNullOrWhiteSpace(SearchText)
            ? new ObservableRangeCollection<Topic>(_topicRepository.Topics)
            : new ObservableRangeCollection<Topic>(TopicList.Where(t => t.Name.Contains(SearchText)));
    }

    private void AddTopic(Topic topic)
    {
        var tempList = TopicList.ToList();
        tempList.Add(topic);

        TopicList = new ObservableRangeCollection<Topic>(tempList);
    }

    private async Task OnItemTappedAsync(Topic topic)
    {
        if (IsNavigate) return;
        if (!topic.IsRoomFilled)
        {
            IsNavigate = true;
            var parameters = new NavigationParameters { { "TopicId", topic.Id } };
            await ChatService.LogInChatAsync(Settings.UserCode, topic.Id);
            await NavigationService.NavigateAsync(Settings.MessagesPageNavigation, parameters, true);
        }

        ShowMessage("This room is filled. Choose an another room or create your own room! :)", 2000);
    }

    private async Task NavigateToAdd()
    {
        if (IsNavigate) return;
        IsNavigate = true;
        await NavigationService.NavigateAsync(Settings.AddTopicNavigation, null, true);
    }

    public override async void OnNavigatedTo(INavigationParameters parameters)
    {
        IsNavigate = false;
        PageMessage = await TryConnectAsync();
    }

    public void UpdateTopic(int topicId, int countUsers, System.DateTime lastEntry)
    {
        if (Topic.Id != topicId)
        {
            ShowMessage("Something wrong with topic updating...");
        }

        var tempTopicList = TopicList;

        Topic = tempTopicList.SingleOrDefault(t => t.Id == topicId);
        Topic.CurrentUsersNumber = countUsers;
        Topic.LastEntryDate = lastEntry;

        TopicList = new ObservableRangeCollection<Topic>(tempTopicList);
    }

    public async Task UpdateTopicAsync(int topicId, System.DateTime lastEntry, string userCode, Color color)
    {
        if (Topic.Id != topicId)
        {
            ShowMessage("Something wrong with topic connection...");
        }

        var tempTopicList = TopicList;

        Topic = tempTopicList.Single(t => t.Id == topicId);
        Topic.LastEntryDate = lastEntry;

    }

    private void SetIsFavorite(string topicName)
    {
        var topicForUpdate = TopicList.First(t => t.Name.Equals(topicName));
        topicForUpdate.IsFavorite = !topicForUpdate.IsFavorite;

        ICollection<string> topicNames = (from t in TopicList where t.IsFavorite select t.Name).ToList();

        Settings.SaveCollection(topicNames, "favoriteTopics");
    }

    private void LoadFavorites()
    {
        var favoriteTopicNames = Settings.GetCollection("favoriteTopics");

        if (favoriteTopicNames == null) return;

        foreach (var name in favoriteTopicNames)
        {
            for (int i = 0; i < TopicList.Count; i++)
            {
                if (name.Equals(TopicList[i].Name))
                {
                    TopicList[i].IsFavorite = true;
                    break;
                }
            }
        }
    }

    private async Task FilterOptionChanged(string categoryName)
    {
        IsBusy = true;
        if (string.IsNullOrEmpty(categoryName))
        {
            TopicList = new ObservableRangeCollection<Topic>(_topicRepository.Topics);
        }
        else
        {
            var tempList = new ObservableRangeCollection<Topic>();

            for (int i = 0; i < TopicList.Count; i++)
            {
                foreach (var category in TopicList[i].Categories)
                {
                    if (category.Split(" ").Contains(categoryName))
                    {
                        tempList.Add(TopicList[i]);
                    }
                }
            }

            TopicList = new ObservableRangeCollection<Topic>(tempList);
        }


        IsBusy = false;
    }

    private void FilterChanged(string filter)
    {
        TopicList = string.IsNullOrWhiteSpace(Filter)
            ? new ObservableRangeCollection<Topic>(_topicRepository.Topics)
            : new ObservableRangeCollection<Topic>(TopicList.Where(t => t.Categories.Contains(Filter)));
    }

    private async void ExecuteSortAsync()
    {
        var sort = await PrismApplicationBase.Current.MainPage.DisplayActionSheet("Sort by", "Cancel", null,
            buttons: new[] { "Name", "Count", "Date" });

        if (sort != "Cancel")
        {
            TopicList = sort switch
            {
                "Name" => new ObservableRangeCollection<Topic>(TopicList.OrderBy(t => t.Name)),
                "Count" => new ObservableRangeCollection<Topic>(TopicList.OrderBy(t => t.MaxUsersNumber)),
                _ => new ObservableRangeCollection<Topic>(TopicList.OrderBy(t => t.LastEntryDate))
            };

            return;
        }

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.Topics);
    }

    private string SortBy
    {
        get => _sortBy;
        set => this.RaiseAndSetIfChanged(ref _sortBy, value);
    }

    public ICommand SortCommand { get; }

    private string _sortBy;


}