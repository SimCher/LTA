using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Helpers;
using MvvmHelpers;
using Prism.Navigation;
using Prism.Services.Dialogs;

namespace LTA.Mobile.PageModels;

public partial class TopicListPageModel
{
    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            await RefreshTopics();
            IsBusy = false;
            Settings.CurrentPage = PageNames.Topics;
            await TryConnectAsync();
            ChatService.UpdateTopic(AddTopicAsync);
            await RefreshTopics();
            LoadFavorites();
        }
        catch (Exception ex)
        {
            await DialogService.ShowDialogAsync($"Error!, {ex.Source}: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshTopics()
    {
        var topics = await _topicRepository.GetAllAsync();
        Debug.WriteLine("Refreshing...");
        TopicList = new ObservableRangeCollection<Topic>(topics);
        IsRefreshing = false;
        
    }

    private async Task OnItemTappedAsync(Topic topic)
    {
        if (IsNavigate) return;
        if (!topic.IsRoomFilled)
        {
            IsNavigate = true;
            var parameters = new NavigationParameters { { "TopicId", topic.Id } };
            await NavigationService.NavigateAsync(Settings.MessagesPageModal, parameters, true);
            IsNavigate = false;
        }
        ShowMessage("This room is filled. Choose an another room or create your own room! :)", 2000);
    }
}