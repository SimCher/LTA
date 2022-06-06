using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface IMessageRepository
{
    ValueTask<Message> GetAsync(int id);
    IEnumerable<Message> GetAll();

    IEnumerable<Message> GetAllMessagesForTopic(int topicId);

    Task AddMessageAsync(Message message);

    Task UpdateMessageAsync(int id, Message message);

    Task<bool> RemoveMessageAsync(int messageId);
}