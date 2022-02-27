using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.Domain.Models;
using LTA.Mobile.ViewModels;

namespace LTA.Mobile.Interfaces;

public interface ITopicRepository
{
    Task<ICollection<Topic>> GetAllAsync();

    Task<Topic> GetAsync(int topicId);
}