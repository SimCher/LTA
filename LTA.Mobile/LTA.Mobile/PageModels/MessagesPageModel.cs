using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Domain.Properties;
using LTA.Mobile.EventHandlers;
using LTA.Mobile.Helpers;
using LTA.Mobile.Resources;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels;

public class MessagesPageModel : BasePageModel
{
    public MessagesPageModel([NotNull] INavigationService navigationService, [NotNull] IChatService chatService,
        ITopicRepository topicRepository, IMessageRepository messageRepository, IPageDialogService dialogService) : base(navigationService, chatService)
    {
        TopicRepository = topicRepository;
        MessageRepository = messageRepository;
        DialogService = dialogService;

        ReplyMessageSelectedCommand = ReactiveCommand.Create<Message>(ReplyMessageSelected);
        MessageSwippedCommand = ReactiveCommand.Create<Message>(MessageSwiped);
        SendMsgCommand = new DelegateCommand(SendMessage);
        UserTypingCommand = new DelegateCommand(SendTyping);
        CancelReplyCommand = ReactiveCommand.Create(CancelReply);

        _messages = new List<Message>();

        //ChatService.NewUserMessage();
    }

    public ICommand SendMsgCommand { get; }

    public ICommand MessageSwippedCommand { get; }

    public ICommand CancelReplyCommand { get; }

    public ICommand ReplyMessageSelectedCommand { get; }

    public ICommand UserTypingCommand { get; }

    public Topic CurrentTopic
    {
        get => _topic;
        set => this.RaiseAndSetIfChanged(ref _topic, value);
    }

    public bool IsTyping
    {
        get => _isTyping;
        set => this.RaiseAndSetIfChanged(ref _isTyping, value);
    }

    public Message ReplyMessage
    {
        get => _replyMessage;
        set => this.RaiseAndSetIfChanged(ref _replyMessage, value);
    }

    public IPageDialogService DialogService { get; }

    public ObservableCollection<MessageGroup> Messages
    {
        get => _messagesGroups;
        set => this.RaiseAndSetIfChanged(ref _messagesGroups, value);
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public void CancelReply()
    {
        ReplyMessage = null;
        MessagingCenter.Send<BasePageModel, LTAFocusEventArgs>(this, "ShowKeyboard",
            new LTAFocusEventArgs { IsFocused = false });
    }

    public override async void Initialize(INavigationParameters parameters)
    {
        try
        {
            ChatService.ReceiveMessage(GetMessage);
            ChatService.ReceiveTyping(UserTyping);
            TopicId = parameters.GetValue<int>("TopicId");
            IsBusy = true;
            CurrentTopic = await TopicRepository.GetAsync(TopicId);

            var messages = MessageRepository.GetAllForTopic(TopicId);

            if (messages != null)
                _messages.AddRange(messages);

            _messages.Add(

                new Message()
                {
                    Id = 1,
                    CreationDate = DateTime.Now,
                    Topic = CurrentTopic,
                    UserCode = Guid.NewGuid().ToString("D"),
                    IsSent = true,
                    Content = "hello",
                    TopicId = CurrentTopic.Id,
                    IsSentPreviousMessage = false
                }
            );

            _messages.Add(

                new Message()
                {
                    Id = 1,
                    CreationDate = DateTime.Now,
                    Topic = CurrentTopic,
                    UserCode = Guid.NewGuid().ToString("D"),
                    IsSent = false,
                    Content = "hello",
                    TopicId = CurrentTopic.Id,
                    IsSentPreviousMessage = false
                }

            );

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

    public override async void OnNavigatedFrom(INavigationParameters parameters)
    {
        await ChatService.LogOutFromChatAsync(Settings.UserCode, CurrentTopic.Id);
    }

    private void MessageSwiped(Message message)
    {
        ReplyMessage = message;
        MessagingCenter.Send<BasePageModel, LTAFocusEventArgs>(this, "ShowKeyboard",
            new LTAFocusEventArgs { IsFocused = true });
    }

    private void ScrollToMessage(Message message)
    {
        MessagingCenter.Send<BasePageModel, ScrollToItemEventArgs>(this, "ScrollToItem",
            new ScrollToItemEventArgs { Item = message });
    }

    private async void SendMessage()
    {
        var isSent = Messages?.Last().Last()?.IsSent;
        var message = new Message
        {
            Content = Message,
            ReplyTo = ReplyMessage,
            CreationDate = DateTime.Now,
            //UserId = int.Parse(Settings.UserId),
            UserCode = Guid.NewGuid().ToString("D"),
            IsSentPreviousMessage = isSent != null && (bool)isSent,
            IsSent = true,
            TopicId = CurrentTopic.Id
        };

        await ChatService.SendMessage(message, message.TopicId);
        AddMessage(message);

        Message = string.Empty;
        ScrollToMessage(Messages.Last().Last());
    }

    private async void AddMessage(Message message)
    {
        Messages.Last().Add(message);
        await MessageRepository.AddMessageAsync(message);
    }

    private async void AddMessage(dynamic message)
    {
        var dynamicMessage = JsonConvert.DeserializeObject(message.ToString());
        if (dynamicMessage != null)
        {
            var replyTo = JsonConvert.DeserializeObject<Message>(dynamicMessage.m.replyTo.ToString()) as Message;

            var isSent = Messages?.Last().Last()?.IsSent;
            var messageObject = new Message
            {
                Id = (int)dynamicMessage.m.id,
                Content = (string)dynamicMessage.m.content,
                ReplyTo = replyTo,
                CreationDate = (DateTime)dynamicMessage.m.creationDate,
                UserCode = (string)dynamicMessage.m.userId,
                IsSentPreviousMessage = isSent != null && (bool)isSent,
                IsSent = false,
                TopicId = CurrentTopic.Id
            };

            Messages?.Last()?.Add(messageObject);

            await MessageRepository.AddMessageAsync(messageObject);
        }
    }

    private void GetMessage(dynamic message)
    {
        AddMessage(message);
        ScrollToMessage(Messages.Last().Last());
    }

    private void ReplyMessageSelected(Message message)
    {
        ScrollToMessage(message);
    }

    private void NetworkDisconnected(string message)
    {
        DialogService.DisplayAlertAsync("Alert", message, "OK");
    }

    private void NewUserMessage()
    {
        ShowMessage("New user is coming", 3000);
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

    private int TopicId { get; set; }

    private ITopicRepository TopicRepository { get; }
    private IMessageRepository MessageRepository { get; }

    private string _message;

    private readonly List<Message> _messages;

    private ObservableCollection<MessageGroup> _messagesGroups;

    private Message _replyMessage;

    private bool _isTyping;

    private Topic _topic;
}