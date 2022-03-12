using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Helpers;
using LTA.Mobile.Pages;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.Mobile.PageModels
{
    public partial class ChatRoomPageModel : BasePageModel
    {
        private readonly IUserService _userService;
        private Topic _topic;

        public Topic CurrentTopic
        {
            get => _topic;
            set => this.RaiseAndSetIfChanged(ref _topic, value);
        }

        private string _message;
        private Message _selectedMessage;
        private readonly List<Message> _messages;
        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.RaiseAndSetIfChanged(ref _isConnected, value);
                });
            }
        }

        public IPageDialogService DialogService;
        public Message SelectedMessage
        {
            get => _selectedMessage;
            set => this.RaiseAndSetIfChanged(ref _selectedMessage, value);
        }
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        private ObservableCollection<MessageGroup> _messageGroups;

        public ObservableCollection<MessageGroup> MessageList
        {
            get => _messageGroups;
            set => this.RaiseAndSetIfChanged(ref _messageGroups, value);
        }

        //private IEnumerable<Message> _messageList;
        //public IEnumerable<Message> MessageList
        //{
        //    get => _messageList;
        //    set => SetProperty(ref _messageList, value);
        //}
        public ICommand SendMsgCommand { get; private set; }
        public ICommand ConfirmNavigateCommand { get; private set; }
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }

        public ChatRoomPageModel(
            INavigationService navigationService, IPageDialogService pageDialog,
            IChatService chatService, IUserService userService)
            : base(navigationService, chatService)
        {
            _userService = userService;
            DialogService = pageDialog;
            InitializeReactive();
            ConnectCommand = ReactiveCommand.CreateFromTask(TryConnectAsync);
            ConfirmNavigateCommand = new DelegateCommand<INavigationParameters>(OnNavigatedFrom);

            _messages = new List<Message>();
        }

        public override async void Initialize(INavigationParameters parameters)
        {

            try
            {
                IsBusy = true;
                CurrentTopic = parameters.GetValue<Topic>("Topic");
                //ChatService.ReceiveMessage(GetMessage);
                ShowMessage(await TryConnectAsync() ?? string.Empty, 3000);
            }
            catch (System.Exception exp)
            {
                NetworkDisconnected($"{exp.Source}: {exp.Message}");
            }

            finally
            {
                IsBusy = false;
            }
        }

        private async Task SendMsg()
        {
            var isSent = MessageList?.Last().Last().IsSent;
            AddMessage(Message, true, Settings.UserId);
            //await ChatService.SendMessage(_userService.GetUserCode(), Message, CurrentTopic.Id);

        }

        private void GetMessage(int userId, string content)
        {
            AddMessage(content, false, userId);
        }

        private async void AddMessage(string content, bool isOwner, int userId)
        {
            var currentUserId = Settings.UserId;
            if (currentUserId != default)
            {
                var message = new Message
                {
                    Content = Message,
                    ReplyTo = ReplyMessage,
                    CreationDate = DateTime.Now,
                    UserId = currentUserId,
                    IsSent = true,
                    TopicId = CurrentTopic.Id,
                    IsOwner = isOwner,
                };

                Message = string.Empty;
                MessageList?.Last().Add(message);
                ReplyMessage = null;
                //await ChatService.SendMessage(Settings.UserId, Message, CurrentTopic.Id);
                ScrollToMessage(message);



            }

        }

        private void SetErrorMessage(string message) => ShowMessage(message, 3000);

        private void NewUserMessage()
        {
            ShowMessage("New user has joined the chat room!", 3000);
        }
        private void UserOutMessage()
        {
            ShowMessage("One user left the chat room!", 3000);
        }

        private void NetworkDisconnected(string message)
        {
            DialogService.DisplayAlertAsync("Alert", message, "OK");
        }

        public void UpdateTopic(int topicId, int countUsers)
        {
            if (CurrentTopic.Id != topicId)
            {
                ShowMessage("Something wrong with topic updating...");
            }


        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await ChatService.LogInChatAsync(Settings.UserId, CurrentTopic.Id);
            //await DialogService.DisplayAlertAsync("Checking...", Topic.CountUsersPresentation, "Ok");
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
            await ChatService.LogOutFromChatAsync(Settings.UserId, CurrentTopic.Id);
            //await DialogService.DisplayAlertAsync("Checking...", Topic.CountUsersPresentation, "Ok");
            await NavigationService.NavigateAsync($"NavigationPage/{nameof(TopicListPage)}", parameters, true);
        }

        private async Task ConnectAsync()
        {
            if (IsConnected) return;
            try
            {
                IsBusy = true;
                await ChatService.Connect();
                await ChatService.LogInChatAsync(Settings.UserId, CurrentTopic.Id);
                IsConnected = true;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}