using System.Collections.Generic;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Interfaces;

public interface IMessageRepository
{
    IEnumerable<Message> GetAllForUser(int userId);

    IEnumerable<Message> GetAllForTopic(int topicId);

    void AddMessage(Message message);
}