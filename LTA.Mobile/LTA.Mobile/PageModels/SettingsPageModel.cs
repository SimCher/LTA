using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Helpers;
using MvvmHelpers;
using Prism.Commands;
using Prism.Navigation;

namespace LTA.Mobile.PageModels;

public class SettingsPageModel : BasePageModel
{
    public ObservableRangeCollection<string> Options { get; }
    public SettingsPageModel(INavigationService navigationService, IChatService chatService) : base(navigationService, chatService)
    {
        Options = new ObservableRangeCollection<string>()
        {
            "Logout"
        };
        Settings.CurrentPage = PageNames.Settings;

        LogoutCommand = new DelegateCommand(Logout);
    }

    public ICommand LogoutCommand { get; }

    public async void Logout()
    {
        Settings.Logout();
        await NavigationService.NavigateAsync(Settings.LoginPageNavigation);
    }
}