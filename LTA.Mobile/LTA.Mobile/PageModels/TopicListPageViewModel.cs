using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LTA.Mobile.Interfaces;
using LTA.Mobile.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

public class TopicListPageViewModel : BaseViewModel
{
    private readonly IChatService _chatService;
    private readonly ITopicRepository _topicRepository;
    private ICollection<TopicViewModel> _topicList;

    public TopicListPageViewModel(INavigationService navigationService, IChatService chatService,
        ITopicRepository topicRepository) : base(navigationService)
    {
        _chatService = chatService;
        _topicRepository = topicRepository;

        ItemTappedCommand = new DelegateCommand(OnItemTapped);
    }

    public ICollection<TopicViewModel> TopicList
    {
        get => _topicList;
        set => SetProperty(ref _topicList, value);
    }

    public ICommand ItemTappedCommand { get; private set; }

    public override async void Initialize(INavigationParameters parameters)
    {
        TopicList = new ObservableCollection<TopicViewModel>();

        try
        {
            await _chatService.Connect();
            RefreshTopics();
        }
        catch (System.Exception ex)
        {
            throw;
        }
    }

    private async void RefreshTopics()
    {
        TopicList = (ICollection<TopicViewModel>)await _topicRepository.GetAllAsync();
    }

    private void GetTopics(TopicViewModel topic)
    {
        AddTopic(topic);
    }

    private void AddTopic(TopicViewModel topic)
    {
        var tempList = TopicList.ToList();
        tempList.Add(topic);

        TopicList = new List<TopicViewModel>(tempList);
    }

    private void NavigateToAddTopicPage()
    {

    }

    private void OnItemTapped()
    {

    }


}