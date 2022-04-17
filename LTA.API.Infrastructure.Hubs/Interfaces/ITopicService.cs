using LTA.API.Domain.Models;
using Xamarin.Forms;

namespace LTA.API.Infrastructure.Hubs.Interfaces;

public interface ITopicService
{
    public IEnumerable<dynamic> GetTopicsDynamic();

    public IEnumerable<object> GetTopicsObject();

    public Topic? GetTopic(int id);

    public Task<Topic> AddTopic(string name, int maxUsers, string categories, string code);
    public Task<Topic> AddUserAndReturnTopic(int topicId, string userCode);
    public Task<Topic> UpdateTopicAsync(int id, string userCode, bool? isUserBeingAdded);
    public Task<Topic> RemoveUserAndReturnTopic(int topicId, string userCode);
    public Dictionary<string?, Color> GetChattersAndColors(Topic topic);
}