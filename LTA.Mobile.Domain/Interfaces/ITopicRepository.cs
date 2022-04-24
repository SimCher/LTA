using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface ITopicRepository
{
    ICollection<Topic> GetAll();

    Task<Topic> GetAsync(int topicId);

    Task AddUserInTopicAsync(User user, int topicId);

    Task<bool> RemoveUserFromTopicAsync(string userCode, int topicId);
}