using LTA.API.Domain.Interfaces;
using LTA.API.Infrastructure.Hubs.Extensions;
using LTA.API.Infrastructure.Hubs.Interfaces;

namespace LTA.API.Infrastructure.Hubs.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;

    public TopicService(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    public IEnumerable<dynamic> GetTopicsDynamic()
        => _topicRepository.GetAll().ToDynamicEnumerable();
}