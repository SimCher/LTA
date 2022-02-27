using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Client.Models;

namespace LTA.Mobile.Client.Services.Interfaces
{
    public interface ITopicDataStore : IDataStore<Topic>
    {
        Task<IEnumerable<Topic>> GetTopicsForUser(string userId);
    }
}