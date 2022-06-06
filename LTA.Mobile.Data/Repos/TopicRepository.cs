using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;
using LTA.Mobile.Data.Context;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;

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
        => ChatService.GetTopicsCountAsync();

    public async Task<bool> RemoveAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveAsync(Topic topic)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(Topic topic)
    {
        throw new NotImplementedException();
    }

    // public async Task<ICollection<Topic>> GetAllAsync()
    // {
    //     DW("Trying to get count from server");
    //     var count = await GetCount();
    //     DW($"Count from server:{count.ToString()}");
    //     var currentCount = Context.Topics.Count();
    //
    //     DW($"Current count is: {currentCount.ToString()}");
    //     if (count == currentCount)
    //     {
    //         DW("Current count is equal to server count. Return topics from context.");
    //         return Context.Topics.ToList();
    //     }
    //     
    //     DW("Done! Adding topics from server in context");
    //     Context.Topics.AddRange(topics);
    //     DW("Done! Asynchronously saving changes");
    //     await Context.SaveChangesAsync();
    //     DW("Task is complete. Return topics from context");
    //     return Context.Topics.ToList();
    //
    // }

    public async Task<ICollection<Topic>> GetAllAsync()
    {
        if (!ChatService.IsConnected)
        {
            return Context.Topics.ToList();
        }
        var currentCount = Context.Topics.Count();
        DW($"Current count: {currentCount.ToString()}");

        IEnumerable<Topic> topics;

        if (currentCount == 0)
        {
            try
            {
                DW("Trying to load all topics from server.");
                topics = await ChatService.LoadTopicsAsync();
                DW("Adding topics from server into context...");
                Context.Topics.AddRange(topics);
                DW("Done! Saving changes...");
                await Context.SaveChangesAsync();
                DW("Saved! Return topics from context");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Source}: {ex.Message}");
            }
            return Context.Topics.ToList();
            
        }
        
        DW("Trying to get count of topics from server...");
        var realCount = await GetCount();
        DW($"Count of topics on server:{realCount.ToString()}");

        if (currentCount == realCount)
        {
            return Context.Topics.ToList();
        }

        try
        {
            DW("The counts is not equals. Get the topic IDs from context");
            var currentTopicsIdList = Context.Topics.Select(t => t.Id);
            DW($"Trying to fetch specific topics from the server.");
            topics = await ChatService.LoadTopicsAsync(currentTopicsIdList.ToList());
            DW($"Done! Received {topics.Count().ToString()} topics from server.");
            DW("Adding topics from server into context...");
            Context.Topics.AddRange(topics);
            DW("Done! Saving changes...");
            await Context.SaveChangesAsync();
            DW("Saved! Return topics from context");
        }
        catch (Exception ex)
        {
            DW($"{ex.Source}:{ex.Message}");
        }
       
        
        return Context.Topics.ToList();
    }

    private void DW(string message)
    {
        Debug.WriteLine(message);
    }

    public ValueTask<Topic> GetAsync(int topicId)
    {
        return Context.Topics.FindAsync(topicId);
    }
}