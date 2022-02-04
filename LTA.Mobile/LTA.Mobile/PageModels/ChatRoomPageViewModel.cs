using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LTA.Mobile.Interfaces;
using LTA.Mobile.Models;
using Prism.Commands;
using Prism.Navigation;

namespace LTA.Mobile.PageModels
{
    public class ChatRoomPageViewModel : BaseViewModel
    {
        private readonly IChatService chatService;

        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }


        private IEnumerable<Message> messageList;
        public IEnumerable<Message> MessageList
        {
            get => messageList;
            set => SetProperty(ref messageList, value);
        }
        public ICommand SendMsgCommand { get; private set; }


        public ChatRoomPageViewModel(
            INavigationService navigationService,
            IChatService chatService)
            : base(navigationService)
        {
            this.chatService = chatService;
            SendMsgCommand = new DelegateCommand(SendMsg);
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            UserName = parameters.GetValue<string>("UserNameId");
            MessageList = new List<Message>();
            try
            {
                chatService.ReceiveMessage(GetMessage);
                await chatService.Connect();
            }
            catch (System.Exception exp)
            {
                throw;
            }

        }

        private void SendMsg()
        {
            chatService.SendMessage(UserName, Message);
            AddMessage(UserName, Message, true);
        }

        private void GetMessage(string userName, string message)
        {
            AddMessage(userName, message, false);
        }


        private void AddMessage(string userName, string message, bool isOwner)
        {
            var tempList = MessageList.ToList();
            tempList.Add(new Message { IsOwner = isOwner, Content = message, UserName = userName });
            MessageList = new List<Message>(tempList);
            Message = string.Empty;
        }

    }
}