using LTA.API.Domain.Models;

namespace LTA.API.Infrastructure.Hubs.Interfaces;

public interface ITopicService
{
    public IEnumerable<dynamic> GetTopicsDynamic();

    public Topic? GetTopic(int id);

    public Task<Topic> AddTopic(string name, int maxUsers, string categories, string code);
    public Task<Topic> AddUserAndReturnTopic(int topicId, string userCode);
    public Task<Topic> RemoveUserAndReturnTopic(int topicId, string userCode);
}