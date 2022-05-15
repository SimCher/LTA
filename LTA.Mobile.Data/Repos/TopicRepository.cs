using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Data.Context;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using Newtonsoft.Json;

namespace LTA.Mobile.Data.Repos;

public class TopicRepository : ITopicRepository
{
    private IChatService ChatService { get; }
    private LtaClientContext Context { get; }

    public TopicRepository(LtaClientContext context, IChatService chatService)
    {
        Context = context;
        ChatService = chatService;
    }

    public async Task AddAsync(Topic topic)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCount()
    {
        return ChatService.GetTopicsCountAsync();
    }

    public async Task<bool> RemoveUserFromTopicAsync(string userCode, int topicId)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Topic>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Topic>> GetAllAsync()
    {
        var count = await GetCount();

        if (count != Context.Topics.Count())
        {
            await InitializeAsync();
        }

        return Context.Topics.ToList();
    }

    public ValueTask<Topic> GetAsync(int topicId)
    {
        return Context.Topics.FindAsync(topicId);
    }

    public async Task AddUserInTopicAsync(User user, int topicId)
    {
        throw new NotImplementedException();
    }

    private async Task InitializeAsync()
    {
        var topics = await GetTopicsAsync();

        var definition = new
        {
            Id = 0,
            UserId = 0,
            Name = string.Empty,
            Rating = .0f,
            MaxUsersNumber = 0,
            LastEntryDate = default(DateTime),
            UserNumber = 0,
            Categories = string.Empty
        };

        var enumerable = topics.ToList();
        if (enumerable.Any())
        {
            var topicsObject = enumerable.Select(topic =>
                    JsonConvert.DeserializeAnonymousType(topic.ToString(), definition))
                .Select(anonTopic =>
                {
                    if (anonTopic == null)
                    {
                        throw new NullReferenceException("anonTopic was null");
                    }
                    return new Topic
                    {
                        Id = anonTopic.Id,
                        OwnerUserId = anonTopic.UserId,
                        Name = anonTopic.Name,
                        Rating = anonTopic.Rating,
                        MaxUsersNumber = anonTopic.MaxUsersNumber,
                        LastEntryDate = anonTopic.LastEntryDate,
                        CurrentUsersNumber = anonTopic.UserNumber,
                        CategoriesArray = anonTopic.Categories
                    };
                });

            await Task.WhenAll
            (
                Context.Topics.AddRangeAsync(topicsObject),
                Context.SaveChangesAsync()
            );
        }
    }

    private Task<IEnumerable<object>> GetTopicsAsync()
    {
        if (!ChatService.IsConnected)
        {
            ChatService.Connect();
        }

        if (!Context.Topics.Any()) return ChatService.LoadTopicsAsync();
        var idList = Context.Topics.Select(t => t.Id);

        return ChatService.LoadTopicsAsync(idList);

    }
}