using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface ITopicRepository
{
    Task<ICollection<Topic>> GetAll();
    Task<ICollection<Topic>> GetAllAsync();

    ValueTask<Topic> GetAsync(int topicId);

    Task AddUserInTopicAsync(User user, int topicId);

    Task AddAsync(Topic topic);

    Task<int> GetCount();

    Task<bool> RemoveUserFromTopicAsync(string userCode, int topicId);
}