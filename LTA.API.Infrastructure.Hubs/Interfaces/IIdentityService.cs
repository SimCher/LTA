namespace LTA.API.Infrastructure.Hubs.Interfaces;

public interface IIdentityService
{
    Task? RegisterAsync(string phoneOrEmail, string password, string confirm, string keyWord);
    Task<string?> LoginAsync(string phoneOrEmail, string password);
}