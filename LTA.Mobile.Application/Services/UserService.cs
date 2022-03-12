using System;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;

namespace LTA.Mobile.Application.Services;

public class UserService : IUserService
{
    private readonly IChatService _chatService;

    public UserService(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task<bool> LoginAsync(string phoneOrEmail, string password, Action<string> setMessage)
    {
        try
        {
            _chatService.SetErrorMessage(setMessage);
            return true;
        }
        catch (Exception ex)
        {
            setMessage(ex.Message);
            return false;
        }
    }
}