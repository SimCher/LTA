using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;

namespace LTA.Mobile.Domain.Interfaces;

public interface ITopicRepository
{
    Task<ICollection<Topic>> GetAllAsync();

    Task<Topic> GetAsync(int topicId);
}