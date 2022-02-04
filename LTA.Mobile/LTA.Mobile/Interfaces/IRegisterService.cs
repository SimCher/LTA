using System;
using System.Threading.Tasks;
using LTA.Mobile.ViewModels;

namespace LTA.Mobile.Interfaces
{
    public interface IRegisterService
    {
        Task<bool> RegisterAsync(string phoneOrEmail, string password, string confirm, Action<string> message);
    }
}