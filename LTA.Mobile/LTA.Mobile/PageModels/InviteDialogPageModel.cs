using System;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using Plugin.Messaging;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Xamarin.Essentials;

namespace LTA.Mobile.PageModels;

public class InviteDialogPageModel : BasePageModel, IDialogAware
{
    public InviteDialogPageModel(INavigationService navigationService, IChatService chatService) 
        : base(navigationService, chatService)
    {
        Title = "Пригласить";

        SendSmsCommand = new DelegateCommand(SendSms);
    }

    public ICommand SendSmsCommand { get; }

    public string PhoneOrEmail
    {
        get => _phoneOrEmail;
        set => SetProperty(ref _phoneOrEmail, value);
    }

    public string TopicName
    {
        get => _topicName;
        set => SetProperty(ref _topicName, value);
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

    private async void SendSms()
    {
        try
        {
            var emailMessenger = CrossMessaging.Current.EmailMessenger;

            if (emailMessenger.CanSendEmail)
            {
                emailMessenger.SendEmail(PhoneOrEmail, "Приглашение в Let's Talk About", 
                    $"Привет! Приглашаю тебя в беседу {TopicName}! Твой код для входа: 042353");
            }
        }
        catch (FeatureNotSupportedException ex)
        {
            await PrismApplicationBase.Current.MainPage.DisplayAlert("Не поддерживается :(",
                "Отправка СМС не поддерживается на вашем устройстве", "Ок");
        }
        catch (Exception ex)
        {
            await PrismApplicationBase.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "Ок");
        }
    }

    public event Action<IDialogParameters> RequestClose;

    private string _phoneOrEmail;
    private string _topicName;
}