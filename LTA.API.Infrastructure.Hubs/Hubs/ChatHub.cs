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
            _loggerService.LogInformation(
                $"Profile with phone/email: {phoneOrEmail} has been successfully registered!");
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
        catch (NullReferenceException ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("Logout");
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("Logout");

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
        catch (NullReferenceException ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
            return string.Empty;
        }
    }
    
    public async Task LogInChatAsync(int topicId)
    {
        var stringTopicId = topicId.ToString();
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, stringTopicId);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
        }
    }

    public async Task LogOutFromChatAsync(int topicId)
    {
        var stringTopicId = topicId.ToString();

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, stringTopicId);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
        }
    }

    public async Task SubscribeToChat(string userCode, int topicId)
    {
        var stringTopicId = topicId.ToString();

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, stringTopicId);
            var topic = await _topicService.AddUserAndReturnTopic(topicId, userCode);

            await Clients.OthersInGroup(stringTopicId)
                .SendAsync("AddUser", userCode, topic.LastEntryDate, topic.Chatters.Count);
        }
        catch (NullReferenceException ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("Logout");
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
        }
    }

    public async Task UnsubscribeFromChat(string userCode, int topicId)
    {
        try
        {
            var topic = await _topicService.RemoveUserAndReturnTopic(topicId, userCode);

            await Clients.OthersInGroup(topicId.ToString()).SendAsync("RemoveUser", topic.Id, userCode);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            await Clients.Caller.SendAsync("SetErrorMessage", ex.Message);
        }
    }

    public async Task SendMessage(dynamic message, int topicId)
    {
        var a = new { m = message };
        await Clients.OthersInGroup(topicId.ToString()).SendAsync("ReceiveMessage", a);
    }

    public IEnumerable<dynamic> LoadTopics()
    {
        return _topicService.GetTopicsDynamic();
    }

    public IEnumerable<object> LoadTopicsAsObject()
    {
        
        return _topicService.GetTopicsObject();
    }

    public IEnumerable<object> LoadSpecificTopics(IEnumerable<int> existTopicsIds)
    {
        return _topicService.GetTopicsObject(existTopicsIds);
    }

    public async Task<int> GetTopicsCount()
    {
        return await _topicService.GetCountOfTopics();
    }

    public async Task SendTyping(int topicId)
    {
        await Clients.OthersInGroup(topicId.ToString()).SendAsync("ReceiveTyping");
    }
}