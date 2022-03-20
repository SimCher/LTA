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
        catch (NullReferenceException ex)
        {
            _loggerService.LogError($"{ex.Source}: {ex.Message}");
            return null;
        }
    }

    public async Task<Topic> AddTopic(string name, int maxUsers, string categories, string code)
    {
        var categoriesArray = categories.Split(' ');

        if (_topicRepository.GetAll().FirstOrDefault(t => t.Name == name) != null)
        {
            throw new Exception($"Topic with name {name} exists already!");
        }

        return await _topicRepository.AddAsync(name, maxUsers, categoriesArray, code);
    }

    public async Task<Topic> AddUserAndReturnTopic(int topicId, int userId)
    {
        return await _topicRepository.UpdateAndReturnAsync(topicId, userId, true)
            ?? throw new ArgumentException("Something went wrong with adding a user to the topic");
    }

    public async Task<Topic> RemoveUserAndReturnTopic(int topicId, int userId)
    {
        return await _topicRepository.UpdateAndReturnAsync(topicId, userId, false)
               ?? throw new ArgumentException("Something went wrong with removing a user from the topic");
    }
}