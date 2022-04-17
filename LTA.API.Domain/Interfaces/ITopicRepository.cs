using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface ITopicRepository
{
    IEnumerable<Topic> GetAll();

    Task<Topic> AddAsync(Topic topic);

    //Task<Topic> UpdateAsync(int id, string userCode, bool isUserBeingAdded);

    //Task<Topic> UpdateAndReturnAsync(int id, string userCode, bool isUserBeingAdded);

    public Task<bool> AddChatterInTopic(int topicId, Chatter chatter);

    public Task<bool> RemoveChatterFromTopic(int topicId, Chatter chatter);

    Task<Topic> UpdateAsync(int id, Topic topic);

    Topic? Get(int id);
}