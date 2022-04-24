using System;
using LTA.Mobile.Application.Interfaces;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using ReactiveUI;

namespace LTA.Mobile.PageModels;

public class ReportDialogPageModel : BasePageModel, IDialogAware
{
    public ReportDialogPageModel(INavigationService navigationService, IChatService chatService) : base(
        navigationService, chatService)
    {
        Title = "Report";
    }

    private string _topicName;
    private string _reportText;

    public string ReportText
    {
        get => _reportText;
        set => this.RaiseAndSetIfChanged(ref _reportText, value);
    }

    public string TopicName
    {
        get => _topicName;
        set => this.RaiseAndSetIfChanged(ref _topicName, value);
    }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
        throw new NotImplementedException();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        TopicName = parameters.GetValue<string>("topicName");
    }
#warning disable
    public event Action<IDialogParameters> RequestClose;
}