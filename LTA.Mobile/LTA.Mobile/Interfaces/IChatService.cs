#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.EventHandlers;

namespace LTA.Mobile.Interfaces
{
    public interface IChatService
    {
        public event EventHandler<MessageEventArgs> OnReceivedMessage;
        public event EventHandler<MessageEventArgs> OnEnteredOrExited;
        public event EventHandler<MessageEventArgs> OnConnectionClosed;
        Task Connect();
        Task Disconnect();
        Task SendMessage(Message message);
        void ReceiveMessage(Action<dynamic> getMessageAndUser);
        void SetErrorMessage(Action<string> getErrorMessage);

        Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword);
        Task<string> LoginAsync(string phoneOrEmail, string password);
        Task<IEnumerable<dynamic>> LoadTopicsAsync();

        Task LogInChatAsync(string userCode, int topicId);
        Task LogOutFromChatAsync(string userCode, int topicId);

        void NewUserMessage(Action<string> showNewUserMessage);
        void UserOutMessage(Action<string> showUserOutMessage);
        void UpdateTopic(Action<int, int, DateTime> updateTopicMethod);
    }
}