using System.Threading.Tasks;

namespace LTA.Mobile.Interfaces;

public interface IUserService
{
    Task<bool> LoginAsync(string phoneOrEmail, string password, System.Action<string> setMessage);
}