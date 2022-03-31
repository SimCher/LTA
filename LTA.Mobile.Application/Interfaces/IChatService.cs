#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Domain.Models;
using Xamarin.Forms;

namespace LTA.Mobile.Application.Interfaces
{
    public interface IChatService
    {
        string CurrentUserCode { get; }
        public event EventHandler<MessageEventArgs> OnReceivedMessage;
        public event EventHandler<MessageEventArgs> OnEnteredOrExited;
        public event EventHandler<MessageEventArgs> OnConnectionClosed;
        Task Connect();
        Task Disconnect();
        Task SendMessage(Message message, int topicId);
        Task AddTopicAsync(string name, int maxUsers, string categories, string code);
        void ReceiveMessage(Action<dynamic> getMessageAndUser);
        void SetErrorMessage(Action<string> getErrorMessage);

        Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword);
        Task LoginAsync(string phoneOrEmail, string password);
        Task<IEnumerable<object>> LoadTopicsAsync();

        Task LogInChatAsync(string userCode, int topicId);
        Task LogOutFromChatAsync(string userCode, int topicId);

        void NewUserMessage(Action<string> showNewUserMessage);
        void UserOutMessage(Action<string> showUserOutMessage);
        Task<int> AddUserInTopic(Func<int, Dictionary<string, Color>, DateTime, Task> addUserMethod);
        void RemoveUserFromTopic(Action<int, string> removeUserMethod);

        Task SendTyping(int topicId);
        void ReceiveTyping(Action setIsTypingMethod);
    }
}