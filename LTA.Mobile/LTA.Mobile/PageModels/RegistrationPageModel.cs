using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Pages.Identity;
using Prism.Commands;
using Prism.Navigation;
using ReactiveUI;

namespace LTA.Mobile.PageModels
{
    public class RegistrationPageModel : BasePageModel
    {
        private readonly IRegisterService _registerService;
        private string _phoneOrEmail;
        private string _password;
        private string? _confirm;

        public RegistrationPageModel(INavigationService navigationService, IRegisterService registerService, IChatService chatService)
            : base(navigationService, chatService)
        {
            _registerService = registerService;

            TryRegisterCommand = new DelegateCommand(NavigateToLoginPage);
            NavigateToLoginPageCommand = new DelegateCommand(NavigateToLogin);
        }

        public string PhoneOrEmail
        {
            get => _phoneOrEmail;
            set => this.RaiseAndSetIfChanged(ref _phoneOrEmail, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string Confirm
        {
            get => _confirm;
            set => this.RaiseAndSetIfChanged(ref _confirm, value);
        }

        public ICommand TryRegisterCommand { get; private set; }
        public ICommand NavigateToLoginPageCommand { get; private set; }

        public override async void Initialize(INavigationParameters parameters)
        {
            try
            {
                IsBusy = true;
                await TryConnectAsync();
            }
            catch
            {
                ShowMessage("Can't connect to the API...", 3000);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void NavigateToLoginPage()
        {
            IsBusy = true;
            ShowMessage("Передаю данные...");
            if (await _registerService.RegisterAsync(PhoneOrEmail, Password, Confirm, SetErrorMessage))
            {
                await NavigationService.NavigateAsync($"NavigationPage/{nameof(LoginPage)}");
            }

            PhoneOrEmail = string.Empty;
            Password = string.Empty;
            Confirm = string.Empty;

            IsBusy = false;
        }

        private async void NavigateToLogin()
        {
            await NavigationService.NavigateAsync($"NavigationPage/{nameof(LoginPage)}");
        }

        private void SetErrorMessage(string message)
        {
            ShowMessage(message);
        }
    }
}