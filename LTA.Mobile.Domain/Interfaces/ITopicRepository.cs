using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface ITopicRepository
{
    ICollection<Topic> GetAllAsync();

    Topic Get(int topicId);

    void AddUserInTopic(User user, int topicId);

    bool RemoveUserFromTopic(string userCode, int topicId);
}