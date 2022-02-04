using System;
using System.Windows.Input;
using LTA.Mobile.Interfaces;
using LTA.Mobile.Pages;
using LTA.Mobile.ViewModels;
using Prism.Commands;
using Prism.Navigation;

namespace LTA.Mobile.PageModels
{
    public class RegistrationPageViewModel : BaseViewModel
    {
        private readonly IRegisterService _registerService;
        private readonly IChatService _chatService;
        private string _message;
        private string _phoneOrEmail;
        private string _password;
        private string? _confirm;

        public RegistrationPageViewModel(INavigationService navigationService, IRegisterService registerService, IChatService chatService)
            : base(navigationService)
        {
            _registerService = registerService;
            _chatService = chatService;

            TryRegisterCommand = new DelegateCommand(NavigateToLoginPage);
            NavigateToLoginPageCommand = new DelegateCommand(NavigateToLogin);
        }

        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public string PhoneOrEmail
        {
            get => _phoneOrEmail;
            set => SetProperty(ref _phoneOrEmail, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Confirm
        {
            get => _confirm;
            set => SetProperty(ref _confirm, value);
        }

        public ICommand TryRegisterCommand { get; private set; }
        public ICommand NavigateToLoginPageCommand { get; private set; }

        public override async void Initialize(INavigationParameters parameters)
        {
            try
            {
                await _chatService.Connect();
            }
            catch
            {
                Message = "Can't connect to the API...";
            }
        }

        private async void NavigateToLoginPage()
        {
            Message = "Receiving the data...";
            if (await _registerService.RegisterAsync(PhoneOrEmail, Password, Confirm, SetErrorMessage))
            {
                Message = "You're successfully pass the registration!";
                await NavigationService.NavigateAsync($"NavigationPage/{nameof(LoginPage)}");
            }

            PhoneOrEmail = string.Empty;
            Password = string.Empty;
            Confirm = string.Empty;
        }

        private async void NavigateToLogin()
        {
            await NavigationService.NavigateAsync($"NavigationPage/{nameof(LoginPage)}");
        }
        
        private void SetErrorMessage(string message)
        {
            Message = message;
        }
    }
}