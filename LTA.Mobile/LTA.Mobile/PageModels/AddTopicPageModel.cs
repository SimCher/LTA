using LTA.Mobile.Application.Interfaces;
using Prism.Navigation;

namespace LTA.Mobile.PageModels;

public class AddTopicPageModel : BasePageModel
{
    public AddTopicPageModel(INavigationService navigationService, IChatService chatService) : base(navigationService,
        chatService)
    {

    }
}