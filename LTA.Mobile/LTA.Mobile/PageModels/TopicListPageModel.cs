﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Helpers;
using LTA.Mobile.Pages.Topics;
using Prism.Navigation;
using MvvmHelpers;
using Prism;
using Prism.Commands;
using Prism.Services.Dialogs;
using ReactiveUI;
using Xamarin.Forms;
using NavigationParameters = Prism.Navigation.NavigationParameters;

namespace LTA.Mobile.PageModels;

public class TopicListPageModel : BasePageModel
{
    private readonly ITopicRepository _topicRepository;
    private ObservableRangeCollection<Topic> _topicList;
    private Topic _topic;
    private bool _isRefreshing;

    public TopicListPageModel(INavigationService navigationService, IChatService chatService,
        ITopicRepository topicRepository, IDialogService pageDialogService) : base(navigationService, chatService)
    {
        IsBusy = true;

        _topicRepository = topicRepository;

        //ItemTappedCommand = new DelegateCommand(OnItemTappedAsync);
        NavigateToAddCommand = ReactiveCommand.CreateFromTask(NavigateToAdd);
        ItemTappedCommand = ReactiveCommand.CreateFromTask<Topic>(OnItemTappedAsync);
        FilterOptionChangedCommand = ReactiveCommand.CreateFromTask<string>(FilterOptionChanged, NotBusyObservable);
        RefreshCommand = ReactiveCommand.Create(RefreshTopics);
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
        IsBusy = false;
    }

    public IDialogService DialogService;

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

    public ICommand SendReportCommand { get; }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            Settings.CurrentPage = PageNames.Topics;
            PageMessage = await TryConnectAsync();
            ChatService.AddUserInTopic(AddUser);
            ChatService.RemoveUserFromTopic(RemoveUser);

            RefreshTopics();

            LoadFavorites();
        }
        catch (System.Exception ex)
        {
            await DialogService.ShowDialogAsync($"Error!, ${ex.Source}: {ex.Message}");
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
                TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
                break;
            case "Favorites":
                TopicList = new ObservableRangeCollection<Topic>(TopicList.Where(t => t.IsFavorite));
                break;
            case "My":
                return;
        }
    }

    private void RefreshTopics()
    {
        if (IsBusy) return;
        IsRefreshing = true;
        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
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
            await _topicRepository.AddUserInTopicAsync(new User
            {
                Code = key,
                Color = value
            }, topicId);
        }

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
    }

    private async Task AddUser(int topicId, Dictionary<string, Color> users, DateTime lastEntry)
    {
        foreach (var (key, value) in users)
        {
            await _topicRepository.AddUserInTopicAsync(new User
            {
                Code = key,
                Color = value
            }, topicId);
        }

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
    }

    private async void RemoveUser(int topicId, string userCode)
    {
        await _topicRepository.RemoveUserFromTopicAsync(userCode, topicId);

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
    }

    private void OnSearchTextChanged()
    {
        TopicList = string.IsNullOrWhiteSpace(SearchText)
            ? new ObservableRangeCollection<Topic>(_topicRepository.GetAll())
            : new ObservableRangeCollection<Topic>(TopicList.Where(t => t.Name.ToLower().Contains(SearchText.ToLower())));
    }

    private void AddTopic(Topic topic)
    {
        var tempList = TopicList.ToList();
        tempList.Add(topic);

        TopicList = new ObservableRangeCollection<Topic>(tempList);
    }

    private async Task OnItemTappedAsync(Topic topic)
    {
        IsBusy = true;
        if (IsNavigate) return;
        if (!topic.IsRoomFilled)
        {
            IsBusy = true;
            IsNavigate = true;
            var parameters = new NavigationParameters { { "TopicId", topic.Id } };
            await ChatService.LogInChatAsync(topic.Id);
            IsBusy = false;
            await NavigationService.NavigateAsync(Settings.MessagesPageModal, parameters, true);
        }
        IsBusy = false;
        ShowMessage("This room is filled. Choose an another room or create your own room! :)", 2000);
    }

    private async Task NavigateToAdd()
    {
        if (IsNavigate) return;
        IsNavigate = true;
        await NavigationService.NavigateAsync($"NavigationPage/{nameof(Add)}", null, true);
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        IsBusy = true;
        IsNavigate = false;
        try
        {
            RefreshTopics();
        }
        catch (Exception ex)
        {
            PageMessage = "Ошибка при получении тем! Сообщение: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
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
            TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
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

    private async void FilterChanged(string filter)
    {
        TopicList = string.IsNullOrWhiteSpace(Filter)
            ? new ObservableRangeCollection<Topic>(_topicRepository.GetAll())
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

        TopicList = new ObservableRangeCollection<Topic>(_topicRepository.GetAll());
    }

    private string SortBy
    {
        get => _sortBy;
        set => this.RaiseAndSetIfChanged(ref _sortBy, value);
    }

    public ICommand SortCommand { get; }

    private string _sortBy;


}