using System;
using System.Threading.Tasks;
using LTA.Mobile.Application.Attributes;
using LTA.Mobile.Application.Interfaces;

namespace LTA.Mobile.Application.Services;

public class RegisterService : IRegisterService
{
    private readonly IChatService _chatService;

    public RegisterService(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, Action<string> setMessage)
    {
        try
        {
            _chatService.SetErrorMessage(setMessage);
            await _chatService.Connect();
            var keyword = PhoneOrEmailAttribute.GetKeyword(phoneOrEmail);

            if (string.IsNullOrEmpty(keyword))
            {
                throw new ArgumentException("Invalid phone number or E-Mail. Please, try again.");
            }

            return await _chatService.RegisterAsync(phoneOrEmail, password, confirm, keyword);
        }
        catch (Exception ex)
        {
            setMessage(ex.Message);
            return false;
        }
    }
}