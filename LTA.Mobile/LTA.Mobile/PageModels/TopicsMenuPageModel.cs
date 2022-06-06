using LTA.Mobile.Application.Interfaces;
using Prism.Navigation;

namespace LTA.Mobile.PageModels;

public class TopicsMenuPageModel : BasePageModel
{
    public TopicsMenuPageModel(INavigationService navigationService, IChatService chatService) : base(navigationService, chatService)
    {
    }
}