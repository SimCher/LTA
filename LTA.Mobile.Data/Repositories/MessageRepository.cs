using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Data.Context;
using LTA.Mobile.Data.Interfaces;
using LTA.Mobile.Domain.Interfaces;
using LTA.Mobile.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LTA.Mobile.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private LtaClientContext Context { get; }
    public MessageRepository(LtaClientContext context)
    {
        Context = context;
    }

    public IEnumerable<Message> GetAllForUser(string userCode)
    {
        return Context.Messages.Where(m => m.UserCode == userCode);
    }

    public IEnumerable<Message> GetAllForTopic(int topicId)
    {
        return Context.Messages.Where(m => m.TopicId == topicId);
    }

    public async Task AddMessageAsync(Message message)
    {
        if (message is not { TopicId: > 0 })
        {
            throw new ArgumentException($"Error with message: Equals:{message}. Topic id: {message?.TopicId}");
        }
        Context.Messages.Add(message);

        await Context.SaveChangesAsync();
    }

    public async Task UpdateMessageAsync(int id, Message message)
    {
        if (id <= 0 || message is not { TopicId: > 0 })
        {
            throw new ArgumentException($"Error when updating: id:{id} message:{message} topicId: {message?.TopicId}");
        }

        if (await Context.Messages.FindAsync(id) == null)
        {
            throw new InvalidOperationException($"Cannot find a message with id: {id}");
        }

        Context.Messages.Add(message);

        await Context.SaveChangesAsync();
    }

    public async Task<bool> RemoveMessageAsync(int messageId)
    {
        if (messageId <= 0)
        {
            return false;
        }

        var msgForRemove = await Context.Messages.FindAsync(messageId);

        if (msgForRemove is null)
        {
            return false;
        }

        Context.Remove(msgForRemove);

        await Context.SaveChangesAsync();
        return true;
    }
}