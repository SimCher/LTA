using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LTA.Mobile.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace LTA.Mobile.Services
{
    public class ChatService : IChatService
    {
        private HubConnection _hubConnection;
        public ChatService()
        {
            _hubConnection = new HubConnectionBuilder().WithUrl($"http://192.168.0.107:8082/lta").Build();
            //_hubConnection = new HubConnectionBuilder().WithUrl($"https://localhost:7240/lta").Build();
        }

        public async Task Connect()
        {
            await _hubConnection.StartAsync();
        }

        public async Task Disconnect()
        {
        https://yandex.ru/search/?text=dynamic+xamarin.android&lr=1095
            await _hubConnection.StopAsync();
        }

        public async Task SendMessage(string userId, string message)
        {
            await _hubConnection.InvokeAsync("SendMessage", userId, message);
        }

        public void ReceiveMessage(Action<string, string> getMessageAndUser)
        {
            _hubConnection.On("ReceiveMessage", getMessageAndUser);
        }

        public async Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword)
        {
            return await _hubConnection.InvokeAsync<bool>("RegisterAsync", phoneOrEmail, password, confirm, keyword);
        }

        public async Task<string> LoginAsync(string phoneOrEmail, string password)
        {
            return await _hubConnection.InvokeAsync<string>("LoginAsync", phoneOrEmail, password);
        }

        public async Task<IEnumerable<dynamic>> LoadTopicsAsync()
            => await _hubConnection.InvokeAsync<IEnumerable<dynamic>>("LoadTopics");

        public void SetErrorMessage(Action<string> getErrorMessage)
        {
            _hubConnection.On("SetErrorMessage", getErrorMessage);
        }

        private static string GetLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().StartsWith("192"))
                    return ip.ToString();
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}