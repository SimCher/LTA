using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Client.Helpers;
using LTA.Mobile.Client.Helpers.EventArgs;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Resources;
using LTA.Mobile.Client.Services.Interfaces;
using LTA.Mobile.Client.ViewModels.Helpers;
using LTA.Mobile.Client.ViewModels.Interfaces;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.Mobile.Client.ViewModels
{
    [QueryProperty("TopicId", "topic_id")]
    public class MessageViewModel : BaseViewModel
    {
        public string TopicId { get; set; }
        private Topic _topic;

        public Topic CurrentTopic
        {
            get => _topic;
            set => this.RaiseAndSetIfChanged(ref _topic, value);
        }

        private bool _isTyping;

        public bool IsTyping
        {
            get => _isTyping;
            set => this.RaiseAndSetIfChanged(ref _isTyping, value);
        }

        private Message _replyMessage;

        public Message ReplyMessage
        {
            get => _replyMessage;
            set => this.RaiseAndSetIfChanged(ref _replyMessage, value);
        }

        private ObservableCollection<MessageGroup> _messageGroup;

        public ObservableCollection<MessageGroup> Messages
        {
            get => _messageGroup;
            set => this.RaiseAndSetIfChanged(ref _messageGroup, value);
        }

        private readonly List<Message> _messages;
        private string _currentMessage;

        public string CurrentMessage
        {
            get => _currentMessage;
            set => this.RaiseAndSetIfChanged(ref _currentMessage, value);
        }

        public ICommand SendMessageCommand { get; private set; }
        public ICommand MessageSwippedCommand { get; private set; }
        public ICommand CancelReplyCommand { get; private set; }
        public ICommand ReplyMessageSelectedCommand { get; private set; }
        public MessageViewModel(IDataStore<User> userDataStore, ITopicDataStore topicDataStore, IMessageDataStore messageDataStore) : base(userDataStore, topicDataStore, messageDataStore)
        {
            ReplyMessageSelectedCommand = ReactiveCommand.Create<Message>(ReplyMessageSelected);
            MessageSwippedCommand = ReactiveCommand.Create<Message>(MessageSwiped);
            SendMessageCommand = ReactiveCommand.CreateFromTask(SendMessage,
                this.WhenAnyValue(vm => vm.CurrentMessage, curm => !string.IsNullOrEmpty(curm)));
            CancelReplyCommand = ReactiveCommand.Create(CancelReply);
            _messages = new List<Message>();
        }

        public void CancelReply()
        {
            ReplyMessage = null;
            MessagingCenter.Send<IViewModel, MyFocusEventArgs>(this, Constants.ShowKeyboard,
                new MyFocusEventArgs { IsFocused = false });
        }



        public override async Task Initialize()
        {
            CurrentTopic = await TopicDataStore.GetItemAsync(TopicId);
            var messages = await MessageDataStore.GetMessagesForTopic(TopicId);

            _messages.AddRange(messages);
            var messageGroups = _messages.GroupBy(m => m.CreationDate.Day)
                .Select(group =>
                {
                    var messageGroup = group.ToList().OrderBy(m => m.CreationDate);
                    var message = messageGroup.First();
                    var date = message.CreationDate.Date;
                    var dayDifferent = DateTime.Now.Day - date.Day;
                    string groupHeader;

                    switch (dayDifferent)
                    {
                        case 0:
                            groupHeader = TextResources.Today;
                            break;
                        case 1:
                            groupHeader = TextResources.Yesterday;
                            break;
                        default:
                            groupHeader = date.ToString("MM-dd-yyyy");
                            break;
                    }

                    return new MessageGroup
                    (
                        date,
                        groupHeader,
                        new ObservableCollection<Message>(messageGroup)
                    );
                })
                .OrderBy(m => m.DateTime.Day)
                .ToList();

            await Task.Delay(TimeSpan.FromSeconds(1));
            Messages = new ObservableCollection<MessageGroup>(messageGroups);

            if (Messages.Any())
                ScrollToMessage(Messages?.Last().Last());
        }

        public override Task Stop()
        {
            return Task.CompletedTask;
        }

        public async Task FakeMessaging()
        {
            var shouldReply = true;

            if (shouldReply)
            {
                ScrollToMessage(Messages.Last().Last());
                IsTyping = true;
                await Task.Delay(TimeSpan.FromSeconds(3));
                var message = new Message
                {
                    Content = "Привет, это простой ответ =)",
                    CreationDate = DateTime.Now,
                    Sender = CurrentTopic.Peer,
                    IsSent = false,
                    TopicId = CurrentTopic.Id,
                    SenderId = CurrentTopic.Peer.Id
                };
                Messages.Last().Add(message);
                CurrentTopic.LastMessage = message;
                await TopicDataStore.UpdateItemAsync(CurrentTopic);

                IsTyping = false;
                ScrollToMessage(message);
                await MessageDataStore.AddItemAsync(message);
            }
        }

        private void ReplyMessageSelected(Message message)
        {
            ScrollToMessage(message);
        }

        private void MessageSwiped(Message message)
        {
            ReplyMessage = message;
            MessagingCenter.Send<IViewModel, MyFocusEventArgs>(this, Constants.ShowKeyboard,
                new MyFocusEventArgs { IsFocused = true });
        }

        private void ScrollToMessage(Message message)
        {
            MessagingCenter.Send<IViewModel, ScrollToItemEventArgs>(this, Constants.ScrollToItem,
                new ScrollToItemEventArgs { Item = message });
        }

        private async Task SendMessage()
        {
            var isSent = Messages?.Last()?.Last()?.IsSent;
            var message = new Message
            {
                Content = CurrentMessage,
                ReplyTo = ReplyMessage,
                CreationDate = System.DateTime.Now,
                Sender = AppLocator.CurrentUser,
                IsSentPreviousMessage = isSent != null && (bool)isSent,
                IsSent = true,
                TopicId = CurrentTopic.Id,
                SenderId = AppLocator.CurrentUserId
            };

            CurrentTopic.LastMessage = message;
            await TopicDataStore.UpdateItemAsync(CurrentTopic);
            CurrentMessage = string.Empty;
            Messages.Last().Add(message);
            ReplyMessage = null;
            await MessageDataStore.AddItemAsync(message);
            CurrentTopic.LastMessage = message;
            ScrollToMessage(message);
            await FakeMessaging();
        }
    }
}