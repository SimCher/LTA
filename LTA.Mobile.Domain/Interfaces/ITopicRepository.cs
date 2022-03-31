using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface ITopicRepository
{
    public ICollection<Topic> Topics { get; }
    Task<ICollection<Topic>> GetAllAsync();

    Task<Topic> GetAsync(int topicId);

    Task AddUserInTopic(User user, int topicId);

    Task<bool> RemoveUserFromTopic(string userCode, int topicId);
}