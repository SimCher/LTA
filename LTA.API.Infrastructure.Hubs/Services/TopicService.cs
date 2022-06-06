using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Hubs.Extensions;
using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Loggers.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.API.Infrastructure.Hubs.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IChatterRepository _chatterRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILoggerService _loggerService;

    public TopicService(ITopicRepository topicRepository, ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        ILoggerService loggerService,
        IChatterRepository chatterRepository)
    {
        _topicRepository = topicRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _chatterRepository = chatterRepository;
        _loggerService = loggerService;
    }

    public IEnumerable<dynamic> GetTopicsDynamic()
        => _topicRepository.GetAll().ToDynamicEnumerable();

    public IEnumerable<object> GetTopicsObject(IEnumerable<int>? existTopicsIds = null)
    {
        if (existTopicsIds is null)
        {
            return  _topicRepository.GetAll().ToObjectEnumerable();
        }

        var topics = _topicRepository.GetAll();

        var needTopics = from topic in topics
            let isNeed = existTopicsIds.Any(id => topic.Id == id)
            where !isNeed
            select topic;

        return needTopics.ToObjectEnumerable();
    }
    
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
        if (_topicRepository.GetAll().FirstOrDefault(t => t.Name == name) != null)
        {
            throw new Exception($"Topic with name {name} exists already!");
        }

        var categoriesObjects = await _categoryRepository.GetAll(categories.Split(' '));
        var userId =  _userRepository.GetIdAsync(code);

        var newTopic = new Topic
        {
            Name = name,
            MaxUsersNumber = maxUsers,
            Categories = new List<Category>(),
            UserId = userId
        };

        AddCategories(newTopic, categoriesObjects);

        return await _topicRepository.AddAsync(newTopic);
    }

    public async Task<Topic> UpdateTopicAsync(int id, string userCode, bool? isUserBeingAdded)
    {
        var topicToUpdate = _topicRepository.Get(id) ??
                            throw new NullReferenceException($"cannot find a topic with id: {id}");

        var user = _userRepository.Get(userCode) ??
                   throw new NullReferenceException($"cannot find chatter for user: {userCode}");

        var chatter = await _chatterRepository.GetAsync(user.Id) ??
                      throw new NullReferenceException($"cannot find chatter with id: {user.Id}");

        if (isUserBeingAdded.HasValue)
        {
            if (isUserBeingAdded.Value)
            {
                topicToUpdate.LastEntryDate = DateTime.Now;
                await AddChatterInTopic(topicToUpdate, chatter);
            }
            else
            {
                if (!await RemoveChatterFromTopic(topicToUpdate, chatter))
                {
                    throw new InvalidOperationException(
                        $"Cannot remove chatter from topic with id: {topicToUpdate.Id}");
                }
            }
        }

        return await _topicRepository.UpdateAsync(id, topicToUpdate);
    }

    public async Task<Topic> AddUserAndReturnTopic(int topicId, string userCode)
    {
        return await UpdateTopicAsync(topicId, userCode, true);
    }

    public async Task<Topic> RemoveUserAndReturnTopic(int topicId, string userCode)
    {
        return await UpdateTopicAsync(topicId, userCode, false);
    }

    public Dictionary<string?, Color> GetChattersAndColors(Topic topic)
    {
        return GetTopicChattersAndColors(topic);
    }

    public static Dictionary<string?, Color> GetTopicChattersAndColors(Topic topic)
    {
        var chattersAndColor = new Dictionary<string, Color>();

        if (topic.Chatters == null) return chattersAndColor;
        foreach (var chatter in topic.Chatters)
        {
            chattersAndColor[chatter.Id.ToString()] = chatter.Color;
        }

        return chattersAndColor;
    }

    private bool HasChatter(Topic topic, Chatter chatter)
    {
        if (topic.Chatters is null)
        {
            return false;
        }

        return topic.Chatters.Contains(chatter);
    }

    private async Task AddChatterInTopic(Topic topic, Chatter chatter)
    {
        if (HasChatter(topic, chatter))
        {
            throw new InvalidOperationException(
                $"Chatter with id: {chatter.Id} is already in topic with id: {topic.Id}");
        }

        if (!await _topicRepository.AddChatterInTopic(topic.Id, chatter))
        {
            throw new InvalidOperationException($"Cannot add chatter in topic!");
        }
    }

    private async Task<bool> RemoveChatterFromTopic(Topic topic, Chatter chatter)
    {
        if (!HasChatter(topic, chatter))
        {
            return false;
        }
        
        return await _topicRepository.RemoveChatterFromTopic(topic.Id, chatter);
    }

    private void AddCategories(Topic topic, ICollection<Category> categories)
    {
        switch (categories.Count)
        {
            case 0:
                return;
            case 1:
                topic.Categories?.Add(categories.First());
                break;
            default:
                {
                    foreach (var category in categories)
                    {
                        topic.Categories?.Add(category);
                    }

                    break;
                }
        }
    }

    public async Task<int> GetCountOfTopics() => await _topicRepository.GetCountAsync();
}