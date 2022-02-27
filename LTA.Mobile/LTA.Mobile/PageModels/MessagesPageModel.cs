using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Domain.Annotations;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.EventHandlers;
using LTA.Mobile.Helpers;
using LTA.Mobile.Interfaces;
using LTA.Mobile.Pages;
using LTA.Mobile.Resources;
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
        SendMsgCommand = ReactiveCommand.CreateFromTask(SendMessage,
            this.WhenAnyValue(vm => vm.Message, curm => !string.IsNullOrEmpty(curm)));
        CancelReplyCommand = ReactiveCommand.Create(CancelReply);
        //BackCommand = new Command(async _ => await NavigationService.NavigateAsync("NavigationPage/TopicsPage"));
        _messages = new List<Message>();
    }

    public ICommand SendMsgCommand { get; private set; }

    public ICommand MessageSwippedCommand { get; private set; }

    public ICommand CancelReplyCommand { get; private set; }

    public ICommand ReplyMessageSelectedCommand { get; private set; }

    public ICommand BackCommand { get; private set; }

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

    public IPageDialogService DialogService { get; private set; }

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
            TopicId = parameters.GetValue<int>("TopicId");
            IsBusy = true;
            CurrentTopic = await TopicRepository.GetAsync(TopicId);
            var messages = MessageRepository.GetAllForTopic(TopicId);

            _messages.AddRange(messages);

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

            await Task.Delay(TimeSpan.FromSeconds(1));
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

    private async Task SendMessage()
    {
        var prevMessage = Messages?.Last()?.Last()?.IsSent;
        var message = new Message
        {
            Content = Message,
            ReplyTo = ReplyMessage,
            CreationDate = DateTime.Now,
            //UserId = int.Parse(Settings.UserId),
            UserId = 100,
            IsSentPreviousMessage = prevMessage ?? false,
            IsSent = true,
            TopicId = CurrentTopic.Id
        };

        Message = string.Empty;
        await ChatService.SendMessage(message);
        Messages.Last().Add(message);
        ScrollToMessage(message);

    }

    private void ReplyMessageSelected(Message message)
    {
        ScrollToMessage(message);
    }

    private void NetworkDisconnected(string message)
    {
        DialogService.DisplayAlertAsync("Alert", message, "OK");
    }

    private int TopicId { get; set; }

    private ITopicRepository TopicRepository { get; }
    private IMessageRepository MessageRepository { get; }

    private string _message;

    private List<Message> _messages;

    private ObservableCollection<MessageGroup> _messagesGroups;

    private Message _replyMessage;

    private bool _isTyping;

    private Topic _topic;
}