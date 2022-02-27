using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Hubs.Extensions;
using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Loggers.Interfaces;

namespace LTA.API.Infrastructure.Hubs.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ILoggerService _loggerService;

    public TopicService(ITopicRepository topicRepository, ILoggerService loggerService)
    {
        _topicRepository = topicRepository;
        _loggerService = loggerService;
    }

    public IEnumerable<dynamic> GetTopicsDynamic()
        => _topicRepository.GetAll().ToDynamicEnumerable();

    public Topic? GetTopic(int id)
    {
        try
        {
            var topic = _topicRepository.Get(id);

            if (topic == null) throw new NullReferenceException($"{topic} was null");

            return topic;
        }
        catch (System.NullReferenceException ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            return null;
        }
    }

    public async Task<Topic> AddUserAndReturnTopic(int topicId, string userCode)
    {
        var topicToUpdate = GetTopicOrThrow(topicId);

        return await _topicRepository.UpdateAndReturnAsync(topicId, topicToUpdate.UserNumber + 1)
            ?? throw new ArgumentException("Something went wrong with adding a user to the topic"); ;


    }

    public async Task<Topic> RemoveUserAndReturnTopic(int topicId, string userCode)
    {
        var topicToUpdate = GetTopicOrThrow(topicId);

        return await _topicRepository.UpdateAndReturnAsync(topicId, topicToUpdate.UserNumber - 1)
               ?? throw new ArgumentException("Something went wrong with removing a user from the topic");
    }

    private Topic GetTopicOrThrow(int topicId) =>
        GetTopic(topicId) ?? throw new NullReferenceException($"There is no topic with id: {topicId}");
}