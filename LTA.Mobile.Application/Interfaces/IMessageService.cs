using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Application.Interfaces;

public interface IMessageService
{
    ValueTask<Message> GetMessage(int id);
    Task<Message> SendMessage(Message message);
    Task<Message> ReceiveMessage(dynamic message, int topicId);
    IEnumerable<Message> GetAllMessagesForTopic(int topicId);
}