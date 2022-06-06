using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Helpers;
using LTA.Mobile.Pages.Topics;
using LTA.Mobile.Resources;
using Prism.Navigation;
using MvvmHelpers;
using Prism;
using Prism.Commands;
using Prism.Services.Dialogs;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "AsyncVoidLambda")]
public partial class TopicListPageModel : BasePageModel
{
    private readonly ITopicRepository _topicRepository;
    private ObservableRangeCollection<Topic> _topicList;
    private Topic _topic;
    private bool _isRefreshing;

    public TopicListPageModel(INavigationService navigationService, IChatService chatService,
        ITopicRepository topicRepository, IDialogService pageDialogService) : base(navigationService, chatService)
    {
        Debug.WriteLine("In topiclist constructor");
        _topicRepository = topicRepository;

        NavigateToAddCommand = new DelegateCommand(async () => await NavigateToAdd());
        ItemTappedCommand = new DelegateCommand<Topic>(async (topic) => await OnItemTappedAsync(topic));
        FilterOptionChangedCommand =
            new DelegateCommand<string>(async (filterName) => await FilterOptionChanged(filterName));
        RefreshCommand = new DelegateCommand(async () => await RefreshTopics());
        // FilterOptionChangedCommand = ReactiveCommand.CreateFromTask<string>(FilterOptionChanged);
        // RefreshCommand = ReactiveCommand.CreateFromTask(RefreshTopics);
        OnSearchTextChangedCommand = new DelegateCommand(OnSearchTextChanged);
        FilterChangedCommand = new DelegateCommand<string>(FilterChanged);
        SortCommand = new DelegateCommand(ExecuteSortAsync);
        SetIsFavoriteCommand = new DelegateCommand<string>(SetIsFavorite);

        DialogService = pageDialogService;



        SendReportCommand = new DelegateCommand<string>((topicName) =>
        {
            var parameters = new DialogParameters { { "topicName", topicName } };
            DialogService.ShowDialog("Report", parameters);
        });
    }

    private IDialogService DialogService;

    private bool IsNavigate { get; set; }
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public Topic Topic
    {
        get => _topic;
        set => SetProperty(ref _topic, value);
    }

    public ObservableRangeCollection<Topic> TopicList
    {
        get => _topicList;
        set => SetProperty(ref _topicList, value);
    }

    public string SearchText { get; set; }
    public string Filter { get; set; }

    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            SetProperty(ref _selectedFilter, value);
            ChangeFilter(value);
        }
    }

    private string _selectedFilter;

    public ICommand ItemTappedCommand { get; set; }
    public ICommand NavigateToAddCommand { get; }
    public ICommand FilterOptionChangedCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand OnSearchTextChangedCommand { get; }
    public ICommand FilterChangedCommand { get; }
    public ICommand SetIsFavoriteCommand { get; }

    public ICommand SendReportCommand { get; }

    private async void ChangeFilter(string filter)
    {
        switch (filter)
        {
            case "All":
                var topics = await _topicRepository.GetAllAsync();
                TopicList = new ObservableRangeCollection<Topic>(topics);
                break;
            case "Favorites":
                TopicList = new ObservableRangeCollection<Topic>(TopicList.Where(t => t.IsFavorite));
                break;
            case "My":
                return;
            case "Приватные":
                    TopicList = new ObservableRangeCollection<Topic>(TopicList.Where(t =>
                        t.Categories.Contains("Приватный")));
                    break;
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void GetTopics(Topic topic)
    {
        AddTopic(topic);
    }

    private async void UpdateTopic(int topicId, Dictionary<string, Color> users, DateTime lastEntry)
    {
        var topics = await _topicRepository.GetAllAsync();
        TopicList = new ObservableRangeCollection<Topic>(topics);
    }

    private async void OnSearchTextChanged()
    {
        var topics = await _topicRepository.GetAllAsync();
        TopicList = string.IsNullOrWhiteSpace(SearchText)
            ? new ObservableRangeCollection<Topic>(topics)
            : new ObservableRangeCollection<Topic>(
                TopicList.Where(t => t.Name.ToLower().Contains(SearchText.ToLower())));
    }

    private void AddTopic(Topic topic)
    {
        var tempList = TopicList.ToList();
        tempList.Add(topic);

        TopicList = new ObservableRangeCollection<Topic>(tempList);
    }

    private async Task NavigateToAdd()
    {
        if (IsNavigate) return;
        IsNavigate = true;
        await NavigationService.NavigateAsync($"NavigationPage/{nameof(Add)}", null, true);
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        IsNavigate = false;
    }

    public void UpdateTopic(int topicId, int countUsers, DateTime lastEntry)
    {
        
        if (Topic.Id != topicId)
        {
            ShowMessage("Something wrong with topic updating...");
        }

        var tempTopicList = TopicList;
        
        Topic = tempTopicList.SingleOrDefault(t => t.Id == topicId);
        if (Topic != null)
        {
            Topic.CurrentUsersNumber = countUsers;
            Topic.LastEntryDate = lastEntry;
        }

        TopicList = new ObservableRangeCollection<Topic>(tempTopicList);
    }

    private async void AddTopicAsync(Topic topic)
    {
        await _topicRepository.AddAsync(topic);

        TopicList.Add(topic);
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
            foreach (var t in TopicList)
            {
                if (!name.Equals(t.Name)) continue;
                t.IsFavorite = true;
                break;
            }
        }
    }

    private async Task FilterOptionChanged(string categoryName)
    {
        IsBusy = true;
        if (string.IsNullOrEmpty(categoryName))
        {
            var topics = await _topicRepository.GetAllAsync();
            TopicList = new ObservableRangeCollection<Topic>(topics);
        }
        else
        {
            var tempList = new ObservableRangeCollection<Topic>();

            foreach (var t in TopicList)
            {
                foreach (var category in t.Categories)
                {
                    if (category.Split(" ").Contains(categoryName))
                    {
                        tempList.Add(t);
                    }
                }
            }

            TopicList = new ObservableRangeCollection<Topic>(tempList);
        }


        IsBusy = false;
    }

    private async void FilterChanged(string filter)
    {
        var topics = await _topicRepository.GetAllAsync();
        TopicList = string.IsNullOrWhiteSpace(Filter)
            ? new ObservableRangeCollection<Topic>(topics)
            : new ObservableRangeCollection<Topic>(TopicList.Where(t => t.Categories.Contains(Filter)));
    }

    private async void ExecuteSortAsync()
    {
        IsBusy = true;
        var topics = await _topicRepository.GetAllAsync();
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

        TopicList = new ObservableRangeCollection<Topic>(topics);

        IsBusy = false;
    }

    protected override void ChatService_ConnectionMessage(object sender, ConnectionMessageEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Message))
        {
            e.Message = TextResources.Messages;
        }
        
        base.ChatService_ConnectionMessage(sender, e);
    }

    private string SortBy
    {
        get => _sortBy;
        set => SetProperty(ref _sortBy, value);
    }

    public ICommand SortCommand { get; }

    private string _sortBy;
}