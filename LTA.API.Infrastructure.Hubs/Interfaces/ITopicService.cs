using LTA.API.Domain.Models;

namespace LTA.API.Infrastructure.Hubs.Interfaces;

public interface ITopicService
{
    public IEnumerable<dynamic> GetTopicsDynamic();

    public Topic? GetTopic(int id);

    public Task<Topic> AddUserAndReturnTopic(int topicId, int userId);
    public Task<Topic> RemoveUserAndReturnTopic(int topicId, int userId);
}