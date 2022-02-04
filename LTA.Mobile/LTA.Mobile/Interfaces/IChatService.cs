using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LTA.Mobile.Interfaces
{
    public interface IChatService
    {
        Task Connect();
        Task Disconnect();
        Task SendMessage(string userId, string message);
        void ReceiveMessage(Action<string, string> getMessageAndUser);
        void SetErrorMessage(Action<string> getErrorMessage);

        Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword);
        Task<string> LoginAsync(string phoneOrEmail, string password);
        Task<IEnumerable<dynamic>> LoadTopicsAsync();
    }
}