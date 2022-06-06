using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface ITopicRepository
{
    Task<ICollection<Topic>> GetAllAsync();

    ValueTask<Topic> GetAsync(int topicId);

    Task AddAsync(Topic topic);

    Task<int> GetCount();

    Task<bool> RemoveAsync(int id);
    Task<bool> RemoveAsync(Topic topic);

    Task<bool> UpdateAsync(int id);
    Task<bool> UpdateAsync(Topic topic);
}