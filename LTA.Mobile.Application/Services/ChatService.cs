#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.EventHandlers;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Application.Resources;
using LTA.Mobile.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace LTA.Mobile.Application.Services
{
    //Обслуживающий класс для общения с сервером
    public class ChatService : IChatService
    {
        private readonly string GlobalIP;
        private readonly string LocalIP = @"http://10.0.2.2:5000/lta";
        private string _connectionIP;
        
        private HubConnection _hubConnection;
        private string _currentUserCode;

        public event EventHandler<ConnectionMessageEventArgs> ConnectionMessage;
        public event EventHandler<ConnectionMessageEventArgs>? OnConnecionClosed;
        

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

#pragma warning disable CS8618
        public ChatService()
#pragma warning restore CS8618
        {
            GlobalIP = $"http://192.168.0.106:8082/lta";
            
            InitializeServer(GlobalIP);
        }

        private void InitializeServer(string ip)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(ip).Build();

            _hubConnection.Closed += async (exception) =>
            {
                var args = new ConnectionMessageEventArgs
                {
                    Message = StringResources.Disconnected
                };
                
                OnConnectionMessage(args);

                IsConnected = false;
                await Task.Delay(new Random().Next(0, 5) * 1000);
                try
                {
                    await Connect();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };
        }

        private ValueTask DestroyServer()
        {
            return _hubConnection.DisposeAsync();
        }
        
        /// <summary>
        /// Подключение к серверу
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            if (IsConnected) return;
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {

                    var args = new ConnectionMessageEventArgs
                    {
                        Message = StringResources.Connection
                    };
                    OnConnectionMessage(args);

                    Debug.WriteLine("Oops... Disconnected! Trying to connect");
                    await _hubConnection.StartAsync();
                    IsConnected = true;

                    args.Message = string.Empty;
                    OnConnectionMessage(args);


                }
            }
            catch (Exception ex)
            {
                var args = new ConnectionMessageEventArgs
                {
                    Message = StringResources.Disconnected
                };
                
                OnConnectionMessage(args);
                
                Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(async t => await Connect());
            }

            
        }

        /// <summary>
        /// Отключение от сервера
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            if (!IsConnected) return;
            var args = new ConnectionMessageEventArgs();
            
            try
            {
                await _hubConnection.DisposeAsync();
                args.Message = StringResources.Disconnected;
                OnConnectionMessage(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

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
            await Connect();

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
            await Connect();
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
            await Connect();

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
            await Connect();
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
            await Connect();
            CurrentUserCode = await _hubConnection.InvokeAsync<string>("LoginAsync", phoneOrEmail, password);
        }

        private IEnumerable<Topic> DeserializeTopics(IEnumerable<object> topicsDefinition)
        {
            var definitions = topicsDefinition.ToList();
            var definition = new
            {
                Id = 0,
                UserId = 0,
                Name = string.Empty,
                Rating = .0f,
                MaxUsersNumber = 0,
                LastEntryDate = default(DateTime),
                UserNumber = 0,
                Categories = string.Empty
            };
            
            var topics = new List<Topic>();

            if (definitions.Count != 0)
            {
                for (int i = 0; i < definitions.Count; i++)
                {
                    var anonTopic = JsonConvert.DeserializeAnonymousType(definitions[i].ToString(), definition)
                                    ?? throw new NullReferenceException("AnonTopic was null");

                    topics.Add(new Topic
                    {
                        Id = anonTopic.Id,
                        OwnerUserId = anonTopic.UserId,
                        Name = anonTopic.Name,
                        Rating = anonTopic.Rating,
                        MaxUsersNumber = anonTopic.MaxUsersNumber,
                        LastEntryDate = anonTopic.LastEntryDate,
                        CurrentUsersNumber = anonTopic.UserNumber,
                        CategoriesArray = anonTopic.Categories
                    });
                }
                
            }

            return topics;
        }

        /// <summary>
        /// Асинхронная загрузка тем
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Topic>> LoadTopicsAsync()
        {
            await Connect();

            var topicsDefinitionEnum = await _hubConnection.InvokeAsync<IEnumerable<object>>("LoadTopicsAsObject") ??
                                       throw new NullReferenceException("Server returns null instead topics");

            return DeserializeTopics(topicsDefinitionEnum);
        }

        public async Task<IEnumerable<Topic>> LoadTopicsAsync(IEnumerable<int> existTopicsIds)
        {
            await Connect();
            Debug.WriteLine("ChatService: speak to the server.");
            var topicsDefinitionEnum =
                await _hubConnection.InvokeAsync<IEnumerable<object>>("LoadSpecificTopics", existTopicsIds) ??
                throw new NullReferenceException("Server returns null instead topics");
            Debug.WriteLine("Deserializing topics...");
            return DeserializeTopics(topicsDefinitionEnum);
        }

        public async Task<int> GetTopicsCountAsync()
        {
            await Connect();
            var topicsCount = await _hubConnection.InvokeAsync<int>("GetTopicsCount");
            return topicsCount;
        }

        public void UpdateTopic(Action<Topic> updateTopicMethod)
        {
            _hubConnection.On("UpdateTopic", updateTopicMethod);
        }
        
        public async Task LogInChatAsync(string userCode, int topicId)
        {
            await Connect();
            await _hubConnection.SendAsync("SubscribeToChat", userCode, topicId);
        }
        
        public async Task LogOutFromChatAsync(int topicId)
        {
            await _hubConnection.SendAsync("LogOutFromChatAsync", topicId.ToString());
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

        public void Logout(Action logoutMethod)
            => _hubConnection.On("Logout", logoutMethod);

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
        public void AddUserInTopic(Action<string, DateTime, int> addUserMethod)
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

        protected virtual void OnConnectionMessage(ConnectionMessageEventArgs e)
        {
            var handler = ConnectionMessage;
            handler?.Invoke(this, e);
        }

        private void GetConnectionIP()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());

            for (int i = 0; i < addresses.Length; i++)
            {
                var stringAddress = addresses[i].ToString();
                if (stringAddress.Substring(0, 7).Equals("192.168"))
                {
                    _connectionIP = stringAddress;
                }
            }
        }
    }
}