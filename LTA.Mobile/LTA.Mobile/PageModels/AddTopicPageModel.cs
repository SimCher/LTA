using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Helpers;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;

namespace LTA.Mobile.PageModels;

public class AddTopicPageModel : BasePageModel
{
    private bool _isValidate;
    private string _topicName;
    private int _maxNumbers;
    private string _categories;

    public string TopicName
    {
        get => _topicName;
        set => this.RaiseAndSetIfChanged(ref _topicName, value);
    }

    public int MaxNumbers
    {
        get => _maxNumbers;
        set => this.RaiseAndSetIfChanged(ref _maxNumbers, value);
    }

    public bool IsValidate
    {
        get => _isValidate;
        set => this.RaiseAndSetIfChanged(ref _isValidate, value);
    }

    public string Categories
    {
        get => _categories;
        set => this.RaiseAndSetIfChanged(ref _categories, value);
    }

    public ICommand AddTopicCommand { get; private set; }

    public AddTopicPageModel(INavigationService navigationService, IChatService chatService) : base(navigationService,
        chatService)
    {
        AddTopicCommand = new DelegateCommand(AddTopic);
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        ChatService.Logout(Logout);
    }

    public void AddTopic()
    {
        ChatService.AddTopicAsync(TopicName, MaxNumbers, Categories, Settings.UserCode);
    }

    public async void Logout()
    {
        Settings.Logout();
        await NavigationService.NavigateAsync(Settings.LoginPageNavigation);
    }
}