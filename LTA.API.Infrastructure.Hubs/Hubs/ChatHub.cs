using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Loggers.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

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


    public async Task SendMessage(string userId, string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", userId, message);
    }

    public IEnumerable<dynamic> LoadTopics()
        => _topicService.GetTopicsDynamic();
}