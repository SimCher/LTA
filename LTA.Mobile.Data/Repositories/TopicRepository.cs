using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Domain.Properties;
using Newtonsoft.Json;

namespace LTA.Mobile.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly IChatService _chatService;
    private ICollection<Topic> Topics { get; set; }

    public TopicRepository(IChatService chatService)
    {
        _chatService = chatService;
    }
    public async Task<ICollection<Topic>> GetAllAsync()
    {
        if (Topics == null)
        {
            await InitializeAsync();
        }

        return Topics;
    }

    [ItemCanBeNull]
    public async Task<Topic> GetAsync(int topicId)
    {
        if (Topics == null)
        {
            await InitializeAsync();
        }

        return Topics.FirstOrDefault(t => t.Id == topicId);
    }

    private async Task InitializeAsync()
    {
        //TODO:: Use Preferences
        var topics = await _chatService.LoadTopicsAsync();

        Topics = topics.Select(topic => JsonConvert.DeserializeObject(topic.ToString()))
            .Select(dynamicTopic => new Topic
            {
                Id = (int)dynamicTopic.id,
                OwnerUserId = (int)dynamicTopic.userId,
                Name = (string)dynamicTopic.name,
                Rating = (float)dynamicTopic.rating,
                MaxUsersNumber = (int)dynamicTopic.maxUsersNumber,
                LastEntryDate = (DateTime)dynamicTopic.lastEntryDate,
                CurrentUsersNumber = (int)dynamicTopic.userNumber,
                Categories = ((string)dynamicTopic.categories).Split(',')
            }).ToList();
        Topics.Add(new Topic
        {
            Id = 1000,
            OwnerUserId = 300,
            Name = "Test room",
            LastEntryDate = DateTime.Now,
            Categories = new[] { "Test rooms" }
        });

    }
}