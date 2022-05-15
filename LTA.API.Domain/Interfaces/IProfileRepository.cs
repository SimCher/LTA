using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface IProfileRepository
{
    IEnumerable<Profile> GetAll();
    Task<Profile?> GetAsync(string phoneOrEmail);
    Task? CreateAsync(string? phone, string? email, string password, string? confirm);
    Task UpdateAsync(int id);
    Task<Profile?>? IsDataValidAsync(string phoneOrEmail, string password);
    Task<bool> TryDeleteAsync(int id);
}