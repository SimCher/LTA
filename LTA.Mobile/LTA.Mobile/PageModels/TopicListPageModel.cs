using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Interfaces;
using LTA.Mobile.Pages;
using LTA.Mobile.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using MvvmHelpers;
using ReactiveUI;

namespace LTA.Mobile.PageModels;

public class TopicListPageModel : BasePageModel
{
    private readonly ITopicRepository _topicRepository;
    private ObservableRangeCollection<Topic> _topicList;
    private Topic _topic;

    private bool IsNavigate { get; set; }

    public TopicListPageModel(INavigationService navigationService, IChatService chatService, IUserService userService,
        ITopicRepository topicRepository, IPageDialogService pageDialogService) : base(navigationService, chatService)
    {
        IsBusy = true;
        _topicRepository = topicRepository;

        //ItemTappedCommand = new DelegateCommand(OnItemTappedAsync);
        ItemTappedCommand = ReactiveCommand.CreateFromTask<Topic>(OnItemTappedAsync);
        FilterOptionChangedCommand = ReactiveCommand.CreateFromTask<bool>(FilterOptionChanged, NotBusyObservable);
        DialogService = pageDialogService;

        IsBusy = false;
    }

    public IPageDialogService DialogService;

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

    public ICommand ItemTappedCommand { get; set; }
    public ICommand FilterOptionChangedCommand { get; private set; }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            PageMessage = await TryConnectAsync();
            ChatService.UpdateTopic(UpdateTopic);

            await RefreshTopics();
        }
        catch (System.Exception ex)
        {
            await DialogService.DisplayAlertAsync($"Error!", $"{ex.Source}: {ex.Message}", "Shit...");
        }

        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshTopics()
    {
        TopicList = new ObservableRangeCollection<Topic>(await _topicRepository.GetAllAsync());
    }

    private void GetTopics(Topic topic)
    {
        AddTopic(topic);
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
            await NavigationService.NavigateAsync($"NavigationPage/MessagesPage", parameters, true);
        }

        ShowMessage("This room is filled. Choose an another room or create your own room! :)", 2000);
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

    private async Task FilterOptionChanged(bool isNotOnline)
    {
        IsBusy = true;
        if (isNotOnline)
        {
            await RefreshTopics();
        }

        IsBusy = false;
    }


}