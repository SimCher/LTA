using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Data.Context;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Domain.Properties;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LTA.Mobile.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly IChatService _chatService;
    private LtaClientContext Context { get; }

    public TopicRepository(LtaClientContext context, IChatService chatService)
    {
        Context = context;
        _chatService = chatService;

        InitializeAsync();
    }

    public ICollection<Topic> GetAllAsync()
    {
        var t = Context.Topics.ToList();
        return t;
    }

    public Topic Get(int topicId)
    {
        return Context.Topics.Find(topicId);
    }

    public void AddUserInTopic(User user, int topicId)
    {
        var topic = Get(topicId);

        topic.UsersIn[user.Code] = user.Color;
    }

    public bool RemoveUserFromTopic(string userCode, int topicId)
    {
        var topic = Get(topicId);

        return topic.UsersIn.Remove(userCode);
    }
    //public async Task<ICollection<Topic>> GetAllAsync()
    //{
    //    if (Topics.Count == 0)
    //    {
    //        await InitializeAsync();

    //        //await Context.Topics.AddRangeAsync(Topics);

    //        //await Context.SaveChangesAsync();
    //    }



    //    return Topics;
    //}

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
        if (!_chatService.IsConnected)
        {
            await _chatService.Connect();
        }

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

        var topics = (await _chatService.LoadTopicsAsync()).ToList();

        var topicsObject = topics.Select(topic => JsonConvert.DeserializeAnonymousType(topic.ToString(), definition))
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
            });

        Context.Topics.AddRange(topicsObject);

        foreach (var topic in Context.Topics)
        {
            Console.WriteLine(topic.Id);
        }

        await Context.SaveChangesAsync();
    }
}