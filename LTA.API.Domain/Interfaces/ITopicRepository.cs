using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface ITopicRepository
{
    IEnumerable<Topic> GetAll();

    Task UpdateAsync(int id, int usersCount, bool isUserBeingAdded);

    Task<Topic> UpdateAndReturnAsync(int id, int userId, bool isUserBeingAdded);

    Topic? Get(int id);
}