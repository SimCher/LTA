using System.Windows.Input;
using LTA.Mobile.Interfaces;
using LTA.Mobile.Pages;
using Prism.Commands;
using Prism.Navigation;

namespace LTA.Mobile.PageModels;

public class LoginPageViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private string _message;
    private string _phoneOrEmail;
    private string _password;

    public LoginPageViewModel(INavigationService navigationService, IUserService userService, IChatService chatService) : base(navigationService)
    {
        _userService = userService;
        _chatService = chatService;

        TryLoginCommand = new DelegateCommand(TryToLogin);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public string PhoneOrEmail { get => _phoneOrEmail; set => SetProperty(ref _phoneOrEmail, value); }

    public string Password { get => _password; set => SetProperty(ref _password, value); }

    public ICommand TryLoginCommand { get; private set; }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            await _chatService.Connect();
        }
        catch
        {
            Message = "Something wrong with server connection...";
        }
    }

    private async void TryToLogin()
    {
        Message = "Logging in...";
        if (await _userService.LoginAsync(PhoneOrEmail, Password, SetErrorMessage))
        {
            await NavigationService.NavigateAsync($"NavigationPage/{nameof(ChatRoomPage)}");
        }

        PhoneOrEmail = string.Empty;
        Password = string.Empty;
    }

    private void SetErrorMessage(string message) => Message = message;

}