using System;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Models;
using Prism.Navigation;
using Prism.Services.Dialogs;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

public class SendPicturePopupDialog : BasePageModel, IDialogAware
{
    
    public SendPicturePopupDialog(INavigationService navigationService, IChatService chatService) : base(navigationService, chatService)
    {
        Title = "Отправить изображение";
    }

    public ImageSource Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public int CurrentTopicId
    {
        get => _currentTopicId;
        set => this.RaiseAndSetIfChanged(ref _currentTopicId, value);
    }

    public string Caption
    {
        get => _caption;
        set => this.RaiseAndSetIfChanged(ref _caption, value);
    }

    public ICommand SendMessage {get;}


    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
        throw new NotImplementedException();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Image = parameters.GetValue<ImageSource>("Image");
        CurrentTopicId = parameters.GetValue<int>("TopicId");
    }

    public event Action<IDialogParameters> RequestClose;

    private ImageSource _image;

    private int _currentTopicId;

    private string _caption;

}