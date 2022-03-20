using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAll();
    Task<Category> AddAsync(string name);
    Task UpdateAsync(int id, string name);
    Task<Category> UpdateAndReturnAsync(int id, string name);
    Category? Get(int id);
    bool Find(string name);

    Task<ICollection<Category>> GetAll(string[] categories);
}