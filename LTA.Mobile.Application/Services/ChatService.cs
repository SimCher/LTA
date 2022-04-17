#nullable enable
using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace LTA.Mobile.Application.Services
{
    //Обслуживающий класс для общения с сервером
    public class ChatService : IChatService
    {
        public event EventHandler<MessageEventArgs> OnReceivedMessage;
        public event EventHandler<MessageEventArgs> OnEnteredOrExited;
        public event EventHandler<MessageEventArgs> OnConnectionClosed;

        private const string GlobalIP = $"http://192.168.168.1:8082/lta";

        private readonly HubConnection _hubConnection;
        private string _currentUserCode;
        private Random _random;

        //Текущий идентификатор пользователя
        public string CurrentUserCode
        {
            get
            {
                if (string.IsNullOrEmpty(_currentUserCode))
                {
                    throw new NullReferenceException(nameof(_currentUserCode));
                }

                return _currentUserCode;
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new NullReferenceException(nameof(value));
                }

                _currentUserCode = value;
            }
        }

        public bool IsConnected { get; private set; }


        private Dictionary<string, string> ActiveTopics { get; } = new();
        public ChatService()
        {
            const string localIP = @"http://10.0.2.2:5240/lta";
            _hubConnection = new HubConnectionBuilder().WithUrl(GlobalIP).Build();
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

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            if (IsConnected) return;

            await _hubConnection.StartAsync();
            IsConnected = true;
        }

        /// <summary>
        /// Отключение от сервера
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message">Содержание сообщения</param>
        /// <param name="topicId">Идентификатор темы</param>
        /// <returns></returns>
        public async Task SendMessage(Message message, int topicId)
        {
            if (!IsConnected)
                await ConnectIfNotAsync();

            await _hubConnection.InvokeAsync("SendMessage", message, topicId);
        }

        /// <summary>
        /// Асинхронное добавление темы для общения
        /// </summary>
        /// <param name="name">Название темы</param>
        /// <param name="maxUsers">Максимальное количество пользователей</param>
        /// <param name="categories">Категории, в которые входит тема</param>
        /// <param name="code">Идентификатор пользователя</param>
        /// <returns></returns>
        public async Task AddTopicAsync(string name, int maxUsers, string categories, string code)
        {
            await ConnectIfNotAsync();
            await _hubConnection.InvokeAsync("AddTopicAsync", name, maxUsers, categories, code);
        }

        /// <summary>
        /// Получение сообщения
        /// </summary>
        /// <param name="getMessageAndUser">Логика при получении сообщения</param>
        public void ReceiveMessage(Action<dynamic> getMessageAndUser)
        {
            _hubConnection.On("ReceiveMessage", getMessageAndUser);
        }

        /// <summary>
        /// Отправка сигнала "печатает"
        /// </summary>
        /// <param name="topicId">Идентификатор темы</param>
        /// <returns></returns>
        public async Task SendTyping(int topicId)
        {
            if (!IsConnected)
                await ConnectIfNotAsync();

            await _hubConnection.InvokeAsync("SendTyping", topicId);
        }

        /// <summary>
        /// Получение сигнала "печатает"
        /// </summary>
        /// <param name="setIsTypingMethod">Логика получения "печатает"</param>
        public void ReceiveTyping(Action setIsTypingMethod)
        {
            _hubConnection.On("ReceiveTyping", setIsTypingMethod);
        }

        /// <summary>
        /// Асинхронная регистрация нового пользователя
        /// </summary>
        /// <param name="phoneOrEmail">Телефон или E-Mail</param>
        /// <param name="password">Пароль</param>
        /// <param name="confirm">Подтверждение  пароля</param>
        /// <param name="keyword">Слово для идентификации (почта или емэйл)</param>
        /// <returns></returns>
        public async Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyword)
        {
            await ConnectIfNotAsync();
            return await _hubConnection.InvokeAsync<bool>("RegisterAsync", phoneOrEmail, password, confirm, keyword);
        }

        /// <summary>
        /// Асихронная аутентификация пользователя
        /// </summary>
        /// <param name="phoneOrEmail">Телефон или E-Mail</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public async Task LoginAsync(string phoneOrEmail, string password)
        {
            await ConnectIfNotAsync();
            CurrentUserCode = await _hubConnection.InvokeAsync<string>("LoginAsync", phoneOrEmail, password);
        }

        /// <summary>
        /// Асинхронная загрузка тем
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<object>> LoadTopicsAsync()
        {
            if (!IsConnected)
            {
                await ConnectIfNotAsync();
            }


            return await _hubConnection.InvokeAsync<IEnumerable<object>>("LoadTopicsAsObject");
        }

        /// <summary>
        /// Асинхронный вход в чат
        /// </summary>
        /// <param name="userCode">Идентификатор пользователя</param>
        /// <param name="topicId">Идентификатор темы</param>
        /// <returns></returns>
        public async Task LogInChatAsync(int topicId)
        {
            if (!IsConnected)
            {
                await ConnectIfNotAsync();
            }
            await _hubConnection.SendAsync("LogInChatAsync", topicId);
        }

        /// <summary>
        /// Асинхронный выход из чата
        /// </summary>
        /// <param name="userCode">Идентификатор пользователя</param>
        /// <param name="topicId">Идентификатор темы</param>
        /// <returns></returns>
        public async Task LogOutFromChatAsync(int topicId)
        {
            if (!IsConnected) return;

            await _hubConnection.SendAsync("LogOutFromChatAsync", topicId);
        }

        /// <summary>
        /// Логика сообщения, если новый пользователь вошёл в тему
        /// </summary>
        /// <param name="showNewUserMessage"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void NewUserMessage(Action<string> showNewUserMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Логика сообщения об ошибке
        /// </summary>
        /// <param name="getErrorMessage"></param>
        public void SetErrorMessage(Action<string> getErrorMessage)
        {
            _hubConnection.On("SetErrorMessage", getErrorMessage);
        }

        /// <summary>
        /// Логика сообщения, если другой пользователь вышел из темы
        /// </summary>
        /// <param name="showUserOutMessage"></param>
        public void UserOutMessage(Action<string> showUserOutMessage)
            => _hubConnection.On("UserOutMessage", showUserOutMessage);

        /// <summary>
        /// Логика добавления пользователя в тему
        /// </summary>
        /// <param name="addUserMethod"></param>
        public void AddUserInTopic(Func<int, Dictionary<string, Color>, DateTime, Task> addUserMethod)
        {
            _hubConnection.On("AddUser", addUserMethod);
        }

        /// <summary>
        /// Логика удаления пользователя из темы
        /// </summary>
        /// <param name="removeUserMethod"></param>
        public void RemoveUserFromTopic(Action<int, string> removeUserMethod)
        {
            _hubConnection.On("RemoveUser", removeUserMethod);
        }

        /// <summary>
        /// Асинхронное подключение к серверу, если подключение не установлено
        /// </summary>
        /// <returns></returns>
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
    }
}