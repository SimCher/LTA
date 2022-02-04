using System;
using System.Threading.Tasks;
using LTA.Mobile.Interfaces;

namespace LTA.Mobile.Services;

public class UserService : IUserService
{
    private string _userCode;
    private readonly IChatService _chatService;

    public UserService(IChatService chatService)
    {
        _chatService = chatService;
    }

    public string UserCode
    {
        get => _userCode;
        private set
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 15)
            {
                throw new System.ArgumentException(
                    "Value for user code was null or value's length is less or equal 15");
            }
            
            _userCode = value;
        }
    }


    public async Task<bool> LoginAsync(string phoneOrEmail, string password, Action<string> setMessage)
    {
        try
        {
            _chatService.SetErrorMessage(setMessage);
            await _chatService.Connect();

            UserCode = await _chatService.LoginAsync(phoneOrEmail, password);
            return true;
        }
        catch (Exception ex)
        {
            setMessage(ex.Message);
            return false;
        }
    }
}