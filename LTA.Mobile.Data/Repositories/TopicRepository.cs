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
    }

    public async Task<ICollection<Topic>> GetAll()
    {
        if (Context.Topics == null || !Context.Topics.Any())
        {
            await InitializeAsync();
        }
        var t = Context.Topics?.ToList();
        return t;
    }

    public async Task<Topic> GetAsync(int topicId)
    {
        return await Context.Topics.FindAsync(topicId);
    }

    public async Task AddUserInTopicAsync(User user, int topicId)
    {
        var topic = await GetAsync(topicId);

        topic.UsersIn.Add(user);
    }

    public async Task<bool> RemoveUserFromTopicAsync(string userCode, int topicId)
    {
        var topic = await GetAsync(topicId);

        

        return topic.UsersIn.Remove(topic.UsersIn.First(u => u.Code.Equals(userCode)));
    }

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
            UsersIn = new List<User>(),
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

        await Context.Topics.AddRangeAsync(topicsObject);

        foreach (var topic in Context.Topics)
        {
            Console.WriteLine(topic.Id);
        }

        await Context.SaveChangesAsync();
    }
}