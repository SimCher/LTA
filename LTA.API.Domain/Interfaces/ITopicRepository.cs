using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface ITopicRepository
{
    IEnumerable<Topic> GetAll();

    Task<Topic> AddAsync(string name, int maxUsers, string[] categories, string code);

    Task UpdateAsync(int id, string userCode, bool isUserBeingAdded);

    Task<Topic> UpdateAndReturnAsync(int id, string userCode, bool isUserBeingAdded);

    Topic? Get(int id);
}