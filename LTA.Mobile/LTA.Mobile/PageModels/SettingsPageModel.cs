using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Helpers;
using Prism.Navigation;

namespace LTA.Mobile.PageModels;

public class SettingsPageModel : BasePageModel
{
    public SettingsPageModel(INavigationService navigationService, IChatService chatService) : base(navigationService, chatService)
    {
        Settings.CurrentPage = PageNames.Settings;
    }
}