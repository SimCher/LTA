using System.Collections.Generic;
using System.Linq;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.Interfaces;

namespace LTA.Mobile.Repositories;

public class MessageRepository : IMessageRepository
{
    private ICollection<Message> Messages { get; set; }

    public MessageRepository()
    {
        Messages = new List<Message>();
    }
    public IEnumerable<Message> GetAllForUser(int userId)
    {
        return Messages.Where(m => m.Sender.Id == userId);
    }

    public IEnumerable<Message> GetAllForTopic(int topicId)
    {
        return Messages.Where(m => m.Topic.Id == topicId);
    }

    public void AddMessage(Message message)
    {
        if (message != null)
        {
            Messages.Add(message);
        }
    }
}