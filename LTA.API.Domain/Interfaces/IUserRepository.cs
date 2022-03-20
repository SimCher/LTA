using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetAsync(int id);
    Task<int?> GetIdAsync(string code);
    Task CreateAsync(Profile profile);
    Task UpdateAsync(int id);
    Task<User> UpdateAndReturnAsync(int id);
    Task<bool> TryDeleteAsync(int id);
    Task<bool> TryDeleteAsync(string userCode);
}