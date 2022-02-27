using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Client.Models;

namespace LTA.Mobile.Client.Services.Interfaces
{
    public interface IMessageDataStore : IDataStore<Message>
    {
        Task<IEnumerable<Message>> GetMessagesForTopic(string topicId);
    }
}