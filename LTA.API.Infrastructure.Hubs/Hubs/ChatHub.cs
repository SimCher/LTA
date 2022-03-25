using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Loggers.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LTA.API.Infrastructure.Hubs.Hubs;

public class ChatHub : Hub
{
    private readonly ILoggerService _loggerService;
    private readonly IIdentityService _identityService;
    private readonly ITopicService _topicService;

    public ChatHub(ILoggerService loggerService, IIdentityService identityService, ITopicService topicService)
    {
        _loggerService = loggerService;
        _identityService = identityService;
        _topicService = topicService;
    }

    public async Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, string keyWord)
    {
        try
        {
            _loggerService.LogInformation("Try to register new user...");
            await _identityService.RegisterAsync(phoneOrEmail, password, confirm, keyWord);
            _loggerService.LogInformation($"Profile with phone/email: {phoneOrEmail} has been successfully registered!");
            return true;
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
            return false;
        }
    }

    public async Task AddTopicAsync(string name, int maxUsers, string categories, string code)
    {
        try
        {
            var topic = await _topicService.AddTopic(name, maxUsers, categories, code);
            await Clients.All.SendAsync("UpdateTopic", topic);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
        }
    }

    public async Task<string> LoginAsync(string phoneOrEmail, string password)
    {
        try
        {
            _loggerService.LogInformation($"User {phoneOrEmail} is trying to log in...");
            var userCode = await _identityService.LoginAsync(phoneOrEmail, password);
            return userCode == string.Empty
                ? throw new ArgumentException("User get a empty code.")
                : userCode ?? throw new NullReferenceException();
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
            return string.Empty;
        }
    }

    //public async Task<ICollection<string>?> LogInChat(string userCode, int topicId)
    //{
    //    try
    //    {
    //        var topic = _topicService.GetTopic(topicId) ??
    //                    throw new System.NullReferenceException($"Topic is unavailable");
    //        if (topic.ContainsUser(userCode))
    //            throw new ArgumentException($"User {userCode} in topic {topic.Name} already.");

    //        await Groups.AddToGroupAsync(Context.ConnectionId, $"{topic.Id}");

    //        if (!topic.TryAddUser(userCode)) throw new ArgumentException($"{nameof(userCode)} was null or empty.");
    //        await Clients.OthersInGroup($"{topic.Id}").SendAsync("NewUserMessage");

    //        return topic.UsersIn;

    //    }
    //    catch (Exception ex)
    //    {
    //        _loggerService.LogError($"{ex.Source}: {ex.Message}");
    //        await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
    //        return null;
    //    }
    //}

    public async Task LogInChatAsync(string userCode, int topicId)
    {
        try
        {
            var topic = await _topicService.AddUserAndReturnTopic(topicId, userCode);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"{topic.Id}");

            await Clients.OthersInGroup($"{topic.Id}").SendAsync("NewUserMessage");

            await Clients.All.SendAsync("UpdateTopic", topic.Id, topic.UsersIn, topic.LastEntryDate);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
        }
    }

    public async Task LogOutFromChatAsync(string userCode, int topicId)
    {
        try
        {
            var topic = await _topicService.RemoveUserAndReturnTopic(topicId, userCode);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{topic.Id}");

            await Clients.OthersInGroup($"{topic.Id}").SendAsync("UserOutMessage");

            await Clients.All.SendAsync("UpdateTopic", topic.Id, topic.UsersIn);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
        }
    }

    public async Task SendMessage(dynamic message)
    {
        var a = new { m = message };
        await Clients.Others/*($"{message.topicId}")*/.SendAsync("ReceiveMessage", a);
    }

    public IEnumerable<dynamic> LoadTopics()
    {
        return _topicService.GetTopicsDynamic();
    }
}