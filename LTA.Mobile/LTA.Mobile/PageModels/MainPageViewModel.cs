using System.Windows.Input;
using LTA.Mobile.Pages;
using Prism.Commands;
using Prism.Navigation;

namespace LTA.Mobile.PageModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }
        public ICommand NavigateToChatPageCommand { get; private set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            NavigateToChatPageCommand = new DelegateCommand(NavigateToChatPage);
        }

        private void NavigateToChatPage()
        {
            var param = new NavigationParameters { { "UserNameId", UserName } };
            NavigationService.NavigateAsync($"NavigationPage/{nameof(ChatRoomPage)}", param);
        }
    }
}