using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Interfaces;
using LTA.Mobile.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LTA.Mobile.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly IChatService _chatService;

    public TopicRepository(IChatService chatService)
    {
        _chatService = chatService;
    }
    public async Task<IEnumerable<TopicViewModel>> GetAllAsync()
    {
        await _chatService.Connect();

        var topics = await _chatService.LoadTopicsAsync();

        var t = topics.Select(topic => JsonConvert.DeserializeObject(topic.ToString()))
            .Select(dynamicTopic => new TopicViewModel
            {
                TopicId = (int)dynamicTopic.id,
                UserId = (int)dynamicTopic.userId,
                Name = (string)dynamicTopic.name,
                Rating = (float)dynamicTopic.rating,
                MaxUsersNumber = (int)dynamicTopic.maxUsersNumber,
                LastEntryDate = (DateTime)dynamicTopic.lastEntryDate,
                Categories = ((string)dynamicTopic.categories).Split(',')
            }).ToList();

        //TODO:: Split categories pls

        return t;
    }
}