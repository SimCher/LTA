#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;
using Xamarin.Forms;

namespace LTA.Mobile.Application.Interfaces
{
    public interface IChatService
    {
        string CurrentUserCode { get; }
        bool IsConnected { get; }
        //Connection logic
        Task Connect();
        Task Disconnect();
        //Messaging logic
        Task SendMessage(Message message, int topicId);
        void ReceiveMessage(Action<dynamic> getMessageAndUser);
        void SetErrorMessage(Action<string> getErrorMessage);
        void NewUserMessage(Action<string> showNewUserMessage);
        void UserOutMessage(Action<string> showUserOutMessage);
        //Topics logic
        Task AddTopicAsync(string name, int maxUsers, string categories, string code);
        Task<IEnumerable<object>> LoadTopicsAsync();
        Task<IEnumerable<object>> LoadTopicsAsync(IEnumerable<int> existTopicsIds);
        void UpdateTopic(Action<Topic> updateTopicMethod);
        Task LogInChatAsync(string userCode, int topicId);
        Task LogOutFromChatAsync(int topicId);
        void AddUserInTopic(Action<string, DateTime, int> addUserMethod);
        void RemoveUserFromTopic(Action<int, string> removeUserMethod);
        //Identity logic
        Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword);
        Task LoginAsync(string phoneOrEmail, string password);
        //Typing logic
        Task SendTyping(int topicId);
        void ReceiveTyping(Action setIsTypingMethod);
        void Logout(Action logoutMethod);

        Task<int> GetTopicsCountAsync();
    }
}