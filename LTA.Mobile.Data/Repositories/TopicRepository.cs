using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Domain.Properties;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LTA.Mobile.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly IChatService _chatService;
    public ICollection<Topic> Topics { get; private set; }

    public TopicRepository(IChatService chatService)
    {
        _chatService = chatService;
    }
    public async Task<ICollection<Topic>> GetAllAsync()
    {
        await InitializeAsync();
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

    public async Task AddUserInTopic(User user, int topicId)
    {
        var topic = await GetAsync(topicId);

        topic.UsersIn[user.Code] = user.Color;
    }

    public async Task<bool> RemoveUserFromTopic(string userCode, int topicId)
    {
        var topic = await GetAsync(topicId);

        return topic.UsersIn.Remove(userCode);
    }

    //private async Task InitializeAsync()
    //{
    //    //TODO:: Use Preferences
    //    var topics = await _chatService.LoadTopicsAsync();

    //    Topics = topics.Select(topic => JsonConvert.DeserializeObject(topic.ToString()))
    //        .Select(dynamicTopic => new Topic
    //        {
    //            Id = (int)dynamicTopic.id,
    //            OwnerUserId = (int)dynamicTopic.userId,
    //            Name = (string)dynamicTopic.name,
    //            Rating = (float)dynamicTopic.rating,
    //            MaxUsersNumber = (int)dynamicTopic.maxUsersNumber,
    //            LastEntryDate = (DateTime)dynamicTopic.lastEntryDate,
    //            CurrentUsersNumber = (int)dynamicTopic.userNumber,
    //            UsersIn = JsonConvert.DeserializeObject<Dictionary<string, Color>>(dynamicTopic.usersIn.ToString()),
    //            Categories = ((string)dynamicTopic.categories).Split(',')
    //        }).ToList();
    //    Topics.Add(new Topic
    //    {
    //        Id = 1000,
    //        OwnerUserId = 300,
    //        Name = "Test room",
    //        LastEntryDate = DateTime.Now,
    //        CurrentUsersNumber = 1,
    //        MaxUsersNumber = 3,
    //        Categories = new[] { "Test rooms" }
    //    });

    //}
    private async Task InitializeAsync()
    {
        var definition = new
        {
            Id = 0,
            UserId = 0,
            Name = string.Empty,
            Rating = .0f,
            MaxUsersNumber = 0,
            LastEntryDate = default(DateTime),
            UserNumber = 0,
            UsersIn = new Dictionary<string, Color>(),
            Categories = string.Empty
        };

        var topics = await _chatService.LoadTopicsAsync();

        Topics = topics.Select(topic => JsonConvert.DeserializeAnonymousType(topic.ToString(), definition))
            .Select(anonTopic =>
            {
                if (anonTopic == null) throw new NullReferenceException("Cannot deserialize the anonymous type");
                return new Topic
                {
                    Id = anonTopic.Id,
                    OwnerUserId = anonTopic.UserId,
                    Name = anonTopic.Name,
                    Rating = anonTopic.Rating,
                    MaxUsersNumber = anonTopic.MaxUsersNumber,
                    LastEntryDate = anonTopic.LastEntryDate,
                    CurrentUsersNumber = anonTopic.UserNumber,
                    UsersIn = anonTopic.UsersIn,
                    Categories = anonTopic.Categories.Split(',')
                };
            }).ToList();
    }
}