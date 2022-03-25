using System;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Helpers;
using LTA.Mobile.Pages;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;

namespace LTA.Mobile.PageModels;

public class LoginPageModel : BasePageModel
{
    private readonly IUserService _userService;
    private string _message;
    private string _phoneOrEmail;
    private string _password;

    public LoginPageModel(INavigationService navigationService, IUserService userService, IChatService chatService)
        : base(navigationService, chatService)
    {
        _userService = userService;

        TryLoginCommand = new DelegateCommand(TryToLogin);
    }

    public string PhoneOrEmail { get => _phoneOrEmail; set => this.RaiseAndSetIfChanged(ref _phoneOrEmail, value); }

    public string Password { get => _password; set => this.RaiseAndSetIfChanged(ref _password, value); }

    public ICommand TryLoginCommand { get; private set; }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            await ChatService.Connect();
        }
        catch
        {
            ShowMessage("Something wrong with the server connection...");
        }
    }

    private async void TryToLogin()
    {
        ShowMessage("Logging in...");
        if (await _userService.LoginAsync(PhoneOrEmail, Password, SetErrorMessage))
        {
            Settings.UserCode = Guid.NewGuid().ToString("D");
            await NavigationService.NavigateAsync(Settings.TopicsPageNavigation);
        }

        PhoneOrEmail = string.Empty;
        Password = string.Empty;
    }

    private void SetErrorMessage(string message) => ShowMessage(message);

}