using System;
using System.Threading.Tasks;

namespace LTA.Mobile.Application.Interfaces
{
    public interface IRegisterService
    {
        Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, Action<string> message);
    }
}