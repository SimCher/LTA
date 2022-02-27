using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface ITopicRepository
{
    IEnumerable<Topic> GetAll();

    Task UpdateAsync(int id, int usersCount);

    Task<Topic> UpdateAndReturnAsync(int id, int usersCount);

    Topic? Get(int id);
}