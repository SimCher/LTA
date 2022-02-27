//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using LTA.Mobile.Domain.Models;
//using LTA.Mobile.Helpers;
//using LTA.Mobile.Interfaces;
//using Prism.Navigation;
//using Prism.Services;
//using Xamarin.Forms;

//namespace LTA.Mobile.PageModels;

//public class TestChatRoomPageModel : BasePageModel
//{
//    public Message ChatMessage { get; }

//    public ObservableCollection<Message> MessageList { get; }
//    public ObservableCollection<User> UserList { get; }

//    private bool _isConnected;

//    public bool IsConnected
//    {
//        get => _isConnected;
//        set
//        {
//            Device.BeginInvokeOnMainThread(() =>
//            {
//                SetProperty(ref _isConnected, value);
//            });
//        }
//    }

//    private bool _isBusy;
//    public bool IsBusy
//    {
//        get => _isBusy;
//        set => SetProperty(ref _isBusy, value);
//    }

//    public Command SendMessageCommand { get; }
//    public Command ConnectCommand { get; }
//    public Command DisconnectCommand { get; }

//    private Random _random;

//    private IPageDialogService PageDialog { get; }


//    public TestChatRoomPageModel(INavigationService navigationService, IChatService chatService, IPageDialogService pageDialog) : base(navigationService, chatService)
//    {
//        if (DesignMode.IsDesignModeEnabled)
//        {
//            return;
//        }

//        Title = Settings.TopicName;

//        PageDialog = pageDialog;

//        MessageList = new ObservableCollection<Message>();
//        UserList = new ObservableCollection<User>();
//        SendMessageCommand = new Command(async () => await SendMessage());
//        ConnectCommand = new Command(async () => await Connect());
//        DisconnectCommand = new Command(async () => await Disconnect());
//        _random = new Random();

//        ChatService.Connect();

//        ChatService.OnReceivedMessage += (sender, args) =>
//        {
//            SendLocalMessage(args.Message, args.Topic);
//            AddRemoveUser(args.Topic, true);
//        };

//        ChatService.OnEnteredOrExited += (sender, args) =>
//        {
//            AddRemoveUser(args.Topic, args.Message.Contains("entered"));
//        };

//        ChatService.OnConnectionClosed += (sender, args) =>
//        {
//            SendLocalMessage(args.Message, args.Topic);
//        };
//    }

//    private async Task Connect()
//    {
//        if (IsConnected) return;
//        try
//        {
//            IsBusy = true;
//            await ChatService.Connect();
//            await ChatService.LogInChatAsync(Settings.UserCode, int.Parse(Settings.TopicName));
//            IsConnected = true;

//            AddRemoveUser(Settings.UserCode, true);
//            await Task.Delay(500);
//            SendLocalMessage("Connected...", Settings.UserCode);
//        }
//        catch (Exception ex)
//        {
//            SendLocalMessage($"Connection error: {ex.Message}", Settings.UserCode);
//        }
//        finally
//        {
//            IsBusy = false;
//        }
//    }

//    private async Task Disconnect()
//    {
//        if (!IsConnected) return;
//        await ChatService.LogOutFromChatAsync(Settings.UserCode, int.Parse(Settings.TopicName));
//        await ChatService.Disconnect();
//        IsConnected = false;
//        SendLocalMessage("Disconnected...", Settings.UserCode);
//    }

//    private async Task SendMessage()
//    {
//        if (!IsConnected)
//        {
//            await PageDialog.DisplayAlertAsync("Not connected", "Please connect to the server and try again.", "OK");
//            return;
//        }

//        try
//        {
//            IsBusy = true;
//            await ChatService.SendMessage(Settings.UserCode, ChatMessage.Content, int.Parse(Settings.TopicName));

//            ChatMessage.Content = string.Empty;
//        }
//        catch (Exception ex)
//        {
//            SendLocalMessage($"Send failed: {ex.Message}", Settings.UserCode);
//        }
//        finally
//        {
//            IsBusy = false;
//        }
//    }

//    private void SendLocalMessage(string message, string userCode)
//    {
//        Device.BeginInvokeOnMainThread(() =>
//        {
//            var first = UserList.FirstOrDefault(u => u.Code == userCode);

//            MessageList.Insert(0, new Message
//            {
//                Content = message,
//                UserCode = userCode
//            });
//        });
//    }

//    private void AddRemoveUser(string userCode, bool isAdd)
//    {
//        if (string.IsNullOrWhiteSpace(userCode)) return;
//        if (isAdd)
//        {
//            if (UserList.All(u => u.Code != userCode))
//            {
//                Device.BeginInvokeOnMainThread(() =>
//                {
//                    UserList.Add(new User { Code = userCode});
//                });
//            }
//        }
//        else
//        {
//            var user = UserList.FirstOrDefault(u => u.Code == userCode);
//            if (user != null)
//            {
//                Device.BeginInvokeOnMainThread(() =>
//                {
//                    UserList.Remove(user);
//                });
//            }
//        }
//    }
//}