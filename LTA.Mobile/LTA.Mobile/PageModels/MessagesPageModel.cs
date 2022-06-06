using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Domain.Properties;
using LTA.Mobile.EventHandlers;
using LTA.Mobile.Helpers;
using LTA.Mobile.Resources;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.PageModels;

public class MessagesPageModel : BasePageModel
{
    public MessagesPageModel([NotNull] INavigationService navigationService, [NotNull] IChatService chatService,
        ITopicRepository topicRepository, IMessageRepository messageRepository,
        IPageDialogService dialogService,
        IDialogService popupDialog) : base(navigationService, chatService)
    {
        TopicRepository = topicRepository;
        MessageRepository = messageRepository;
        DialogService = dialogService;
        DialogPopup = popupDialog;

        // ReplyMessageSelectedCommand =
        //     new DelegateCommand<Message>(ReplyMessageSelected);
        MessageSwippedCommand =
            new DelegateCommand<Message>(MessageSwiped);
        SendMsgCommand = new DelegateCommand(SendMessage);
        UserTypingCommand = new DelegateCommand(SendTyping);
        CancelReplyCommand = new DelegateCommand(CancelReply);
        SendPictureCommand = new DelegateCommand(SendPictureMessage);
        SelectionChangedCommand = new DelegateCommand<List<Message>>(SelectionChanged);

        if (string.IsNullOrEmpty(Title))
        {
            Title = TextResources.Chat;
        }

        ReplyMessageSelectedCommand = new DelegateCommand<Message>(ScrollToMessage);
        _messages = new List<Message>();
        _selectedMessages = new List<Message>();

        NewMessage = new Message();
        //ChatService.NewUserMessage();

        InviteCommand = new DelegateCommand<string>((topicName) =>
        {
            var parameters = new DialogParameters {{"topicName", topicName}};
            DialogPopup.ShowDialog("Invite", parameters);
        });
    }

    public List<Message> SelectedMessages
    {
        get => _selectedMessages;
        set => SetProperty(ref _selectedMessages, value);
    }

    public ICommand SendMsgCommand { get; }
    public IDialogService DialogPopup { get; }

    public ICommand MessageSwippedCommand { get; }

    public ICommand CancelReplyCommand { get; }

    public ICommand ReplyMessageSelectedCommand { get; }

    public ICommand UserTypingCommand { get; }
    public ICommand SendPictureCommand { get; }
    public ICommand InviteCommand { get; }

    public byte[] Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    public Topic CurrentTopic
    {
        get => _topic;
        set => SetProperty(ref _topic, value);
    }

    public bool IsTyping
    {
        get => _isTyping;
        set => SetProperty(ref _isTyping, value);
    }

    public ICommand SelectionChangedCommand { get; }

    public Message ReplyMessage
    {
        get => _replyMessage;
        set => SetProperty(ref _replyMessage, value);
    }

    public void SelectionChanged(List<Message> sender)
    {
        SelectedMessages = new(sender);
    }

    public bool IsMessagesSelected => SelectedMessages.Count != 0;

    public Message NewMessage { get; private set; }

    private Message _newMessage;

    public IPageDialogService DialogService { get; }

    public ObservableCollection<MessageGroup> Messages
    {
        get => _messagesGroups;
        set => SetProperty(ref _messagesGroups, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public void CancelReply()
    {
        ReplyMessage = null;
        MessagingCenter.Send<BasePageModel, LTAFocusEventArgs>(this, "ShowKeyboard",
            new LTAFocusEventArgs {IsFocused = false});
    }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            ChatService.AddUserInTopic(AddUser);
            ChatService.ReceiveMessage(GetMessage);
            ChatService.ReceiveTyping(UserTyping);
            ChatService.SetErrorMessage(NewUserMessage);
            ChatService.Logout(Logout);
            TopicId = parameters.GetValue<int>("TopicId");

            CurrentTopic = await TopicRepository.GetAsync(TopicId);

            var messages = MessageRepository.GetAllMessagesForTopic(CurrentTopic.Id);

            if (messages != null)
                _messages.AddRange(messages);

            if (_messages.Count == 0)
            {
                _messages.Add(new Message
                {
                    Id = 0,
                    Content = string.Empty,
                    TopicId= CurrentTopic.Id,
                    UserCode = Settings.UserCode
                });
            }
                    

            var messagesGroups = _messages.GroupBy(m => m.CreationDate.Day)
                .Select(group =>
                {
                    var msgGroups = group.OrderBy(m => m.CreationDate);
                    var message = msgGroups.First();
                    var date = message.CreationDate.Date;
                    var dayDifferent = DateTime.Now.Day - date.Day;

                    var groupHeader = dayDifferent switch
                    {
                        0 => TextResources.Today,
                        1 => TextResources.Yesterday,
                        _ => date.ToString("dd-MM-yyyy")
                    };

                    return new MessageGroup(date, groupHeader, new ObservableCollection<Message>(msgGroups));
                })
                .OrderBy(m => m.DateTime.Day).ToList();

            Messages = new ObservableCollection<MessageGroup>(messagesGroups);

            if (Messages.Any())
            {
                ScrollToMessage(Messages?.Last()?.Last());
            }
        }
        catch (Exception ex)
        {
            NetworkDisconnected($"{ex.Source}: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public override async void OnNavigatedTo(INavigationParameters parameters)
    {
        try
        {
            IsBusy = true;
            await ChatService.LogInChatAsync(Settings.UserCode, CurrentTopic.Id);
        }
        catch (Exception ex)
        {
            PageMessage = "Ошибка!";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public override async void OnNavigatedFrom(INavigationParameters parameters)
    {
        await ChatService.LogOutFromChatAsync(CurrentTopic.Id);
    }

    private void MessageSwiped(Message message)
    {
        ReplyMessage = message;
        MessagingCenter.Send<BasePageModel, LTAFocusEventArgs>(this, "ShowKeyboard",
            new LTAFocusEventArgs {IsFocused = true});
    }

    private void ScrollToMessage(Message message)
    {
        MessagingCenter.Send<BasePageModel, ScrollToItemEventArgs>(this, "ScrollToItem",
            new ScrollToItemEventArgs {Item = message});
    }

    public async void Logout()
    {
        Settings.Logout();
        await NavigationService.NavigateAsync(Settings.LoginPageNavigation);
    }

    private async void SendMessage()
    {

        // // NewMessage.EncryptedContent = await CryptoService.EncryptAsync(CryptoService.PublicKey, Message);
        NewMessage.Content = Message;
        NewMessage.CreationDate = DateTime.Now;
        NewMessage.UserCode = Settings.UserCode;
        NewMessage.IsSentPreviousMessage = true;
        NewMessage.IsSent = true;
        NewMessage.TopicId = CurrentTopic.Id;
        NewMessage.ReplyTo = ReplyMessage;

        await ChatService.SendMessage(NewMessage, NewMessage.TopicId);
        AddMessage(NewMessage);
        
        Image = null;
        Message = string.Empty;
        ReplyMessage = null;
        ScrollToMessage(Messages.Last().Last());
        NewMessage = new Message();
    }
    
    private void AddUser(string userCode, DateTime lastEntry, int count)
    {
        CurrentTopic.LastEntryDate = lastEntry;
        CurrentTopic.UsersIn.Add(new User
        {
            Code = userCode
        });
    }
    
    private async void GetMessage(dynamic message)
    {
        AddMessage(message);
        ScrollToMessage(Messages.Last().Last());
    }

    private async void AddMessage(Message message)
    {
        Messages.Last().Add(message);
        try
        {
            await MessageRepository.AddMessageAsync(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{ex.Source}: {ex.Message}");
        }
        
    }
    private async void AddMessage(dynamic message)
    {
        var dynamicMessage = JsonConvert.DeserializeObject(message.ToString());

        if (dynamicMessage is not null)
        {
            var replyTo = JsonConvert.DeserializeObject<Message>(dynamicMessage.m.replyTo.ToString()) as Message;

            var newMessage = new Message
            {
                Id = (int) dynamicMessage.m.id,
                Content = (string) dynamicMessage.m.content,
                Image = dynamicMessage.m.image,
                ReplyTo = replyTo,
                CreationDate = (DateTime) dynamicMessage.m.creationDate,
                UserCode = (string) dynamicMessage.m.userCode,
                IsSent = false,
                TopicId = CurrentTopic.Id
            };
            
            Messages?.Last()?.Add(newMessage);

            try
            {
                await MessageRepository.AddMessageAsync(newMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Source}: {ex.Message}");
            }
            
        }
    }

    private async void SendPictureMessage()
    {
        await CrossMedia.Current.Initialize();

        if (!CrossMedia.Current.IsTakePhotoSupported)
        {
            await DialogService.DisplayAlertAsync("Не поддерживается",
                "В настоящее время ваше устройство не поддерживает эту функцию", "ОК");
            return;
        }

        var mediaOptions = new PickMediaOptions()
        {
            PhotoSize = PhotoSize.Medium
        };

        var selectedImage = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
        if (selectedImage is null)
        {
            await DialogService.DisplayAlertAsync("Ошибка",
                "Не удалось получить изображение, пожалуйста, попробуйте еще раз.", "ОК");
            return;
        }

        var memory = new MemoryStream();
        var stream = selectedImage.GetStream();
        await stream.CopyToAsync(memory);
        NewMessage.Image = memory.ToArray();

        //var parameters = new DialogParameters {{"Image", image}, {"TopicId", CurrentTopic.Id}};
        //DialogPopup.ShowDialog("SendPicture", parameters);
    }

    private void ReplyMessageSelected(Message message)
    {
        ScrollToMessage(message);
    }

    private void NetworkDisconnected(string message)
    {
        DialogService.DisplayAlertAsync("Alert", message, "OK");
    }

    private void NewUserMessage(string message)
    {
        ShowMessage(message, 5000);
    }

    private void SendTyping()
    {
        ChatService.SendTyping(CurrentTopic.Id);
    }

    private void UserTyping()
    {
        if (!IsTyping)
        {
            IsTyping = true;
            Observable.Timer(TimeSpan.FromMilliseconds(1500)).Subscribe(_ => IsTyping = false);
        }
    }

    protected override void ChatService_ConnectionMessage(object sender, ConnectionMessageEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Message))
        {
            e.Message = TextResources.Users;
        }

        base.ChatService_ConnectionMessage(sender, e);
    }

    private int TopicId { get; set; }

    private ITopicRepository TopicRepository { get; }
    private IMessageRepository MessageRepository { get; }
    private ICryptoService CryptoService { get; }

    private string _message;

    private List<Message> _selectedMessages;

    private readonly List<Message> _messages;

    private ObservableCollection<MessageGroup> _messagesGroups;

    private Message _replyMessage;

    private bool _isTyping;

    private Topic _topic;

    private byte[] _image;
}