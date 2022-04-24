using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Helpers;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;

namespace LTA.Mobile.PageModels;

public class LoginPageModel : BasePageModel
{
    private readonly IUserService _userService;
    private string _phoneOrEmail;
    private string _password;

    public LoginPageModel(INavigationService navigationService, IUserService userService, IChatService chatService)
        : base(navigationService, chatService)
    {
        _userService = userService;

        TryLoginCommand = new DelegateCommand(TryToLogin);
        NavigateToRegisterCommand = new DelegateCommand(NavigateToRegister);
    }

    public string PhoneOrEmail { get => _phoneOrEmail; set => this.RaiseAndSetIfChanged(ref _phoneOrEmail, value); }

    public string Password { get => _password; set => this.RaiseAndSetIfChanged(ref _password, value); }

    public ICommand TryLoginCommand { get; private set; }
    public ICommand NavigateToRegisterCommand { get; }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            await ChatService.Connect();
        }
        catch
        {
            ShowMessage("Something wrong with the server connection...");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void TryToLogin()
    {
        ShowMessage("Logging in...");
        IsBusy = true;
        if (await _userService.LoginAsync(PhoneOrEmail, Password, SetErrorMessage))
        {
            Settings.UserCode = ChatService.CurrentUserCode;
            await NavigationService.NavigateAsync(Settings.TopicsPageNavigation);
        }

        PhoneOrEmail = string.Empty;
        Password = string.Empty;
        IsBusy = false;
    }

    private async void NavigateToRegister()
    {
        await NavigationService.NavigateAsync(Settings.RegistrationPageNavigation);
    }

    private void SetErrorMessage(string message) => ShowMessage(message);

}