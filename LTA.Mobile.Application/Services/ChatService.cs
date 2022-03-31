#nullable enable
using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace LTA.Mobile.Application.Services
{
    public class ChatService : IChatService
    {
        public event EventHandler<MessageEventArgs> OnReceivedMessage;
        public event EventHandler<MessageEventArgs> OnEnteredOrExited;
        public event EventHandler<MessageEventArgs> OnConnectionClosed;

        private readonly HubConnection _hubConnection;
        private Random _random;
        public string CurrentUserCode { get; private set; }
        private bool IsConnected { get; set; }
        private Dictionary<string, string> ActiveTopics { get; } = new();
        public ChatService()
        {
            //_hubConnection = new HubConnectionBuilder().WithUrl($"http://192.168.216.1:8082/lta").Build();
            //_hubConnection = new HubConnectionBuilder().WithUrl("http://192.168.234.1:8082/lta").Build();
            _hubConnection = new HubConnectionBuilder().WithUrl(@"http://192.168.216.1:8082/lta").Build();
            _hubConnection.Closed += async (error) =>
            {
                OnConnectionClosed?.Invoke(this, new MessageEventArgs("Connection closed...", string.Empty));
                IsConnected = false;
                await Task.Delay(_random.Next(0, 5) * 1000);
                try
                {
                    await Connect();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };

            _hubConnection.On<string>("Entered", (user) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs($"{user} entered.", user));
            });

            _hubConnection.On<string>("Left", (user) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs($"{user} left.", user));
            });

            _hubConnection.On<string>("EnteredOrLeft", (message) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs(message, message));
            });
        }

        public async Task Connect()
        {
            if (IsConnected) return;

            await _hubConnection.StartAsync();
            IsConnected = true;
        }

        public async Task Disconnect()
        {
            if (!IsConnected) return;

            try
            {
                await _hubConnection.DisposeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            ActiveTopics.Clear();
            IsConnected = false;
        }

        public async Task SendMessage(Message message, int topicId)
        {
            if (!IsConnected)
                await ConnectIfNotAsync();

            await _hubConnection.InvokeAsync("SendMessage", message, topicId);
        }

        public async Task AddTopicAsync(string name, int maxUsers, string categories, string code)
        {
            await ConnectIfNotAsync();
            await _hubConnection.InvokeAsync("AddTopicAsync", name, maxUsers, categories, code);
        }

        public void ReceiveMessage(Action<dynamic> getMessageAndUser)
        {
            _hubConnection.On("ReceiveMessage", getMessageAndUser);
        }

        public async Task SendTyping(int topicId)
        {
            if (!IsConnected)
                await ConnectIfNotAsync();

            await _hubConnection.InvokeAsync("SendTyping", topicId);
        }

        public void ReceiveTyping(Action setIsTypingMethod)
        {
            _hubConnection.On("ReceiveTyping", setIsTypingMethod);
        }

        public async Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword)
        {
            await ConnectIfNotAsync();
            return await _hubConnection.InvokeAsync<bool>("RegisterAsync", phoneOrEmail, password, confirm, keyword);
        }

        public async Task LoginAsync(string phoneOrEmail, string password)
        {
            await ConnectIfNotAsync();
            CurrentUserCode = await _hubConnection.InvokeAsync<string>("LoginAsync", phoneOrEmail, password);
        }

        public async Task<IEnumerable<object>> LoadTopicsAsync()
        {
            await ConnectIfNotAsync();
            return await _hubConnection.InvokeAsync<IEnumerable<object>>("LoadTopicsAsObject");
        }


        //public async Task LogInChatAsync(string userCode, int topicId)
        //{
        //    if (!IsConnected)
        //    {
        //        await ConnectIfNotAsync();
        //    }

        //    await _hubConnection.SendAsync("AddToTopic", topicId, userCode);
        //    ActiveTopics.Add(topicId.ToString(), userCode);
        //    //await ConnectIfNotAsync();
        //    //await _hubConnection.InvokeAsync("LogInChatAsync", userCode, topicId);
        //}

        public async Task LogInChatAsync(string userCode, int topicId)
        {
            if (!IsConnected)
            {
                await ConnectIfNotAsync();
            }

            await _hubConnection.SendAsync("LogInChatAsync", userCode, topicId);

        }



        public async Task LogOutFromChatAsync(string userCode, int topicId)
        {
            if (!IsConnected) return;

            await _hubConnection.SendAsync("LogOutFromChatAsync", userCode, topicId);
        }

        public void SetErrorMessage(Action<string> getErrorMessage)
        {
            _hubConnection.On("SetErrorMessage", getErrorMessage);
        }

        public void NewUserMessage(Action<string> showNewUserMessage)
            => _hubConnection.On("NewUserMessage", showNewUserMessage);

        public void UserOutMessage(Action<string> showUserOutMessage)
            => _hubConnection.On("UserOutMessage", showUserOutMessage);

        public Task<int> AddUserInTopic(Func<int, Dictionary<string, Color>, DateTime, Task> addUserMethod)
        {
            _hubConnection.On("AddUser", addUserMethod);

            return Task.FromResult(1);
        }

        public void RemoveUserFromTopic(Action<int, string> removeUserMethod)
        {
            _hubConnection.On("RemoveUser", removeUserMethod);
        }

        private async Task ConnectIfNotAsync()
        {
            await _hubConnection.StopAsync();
            IsConnected = false;
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                IsConnected = true;
            }
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