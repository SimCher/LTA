using System.Collections.Generic;
using System.Threading.Tasks;
using LTA.Mobile.ViewModels;

namespace LTA.Mobile.Interfaces;

public interface ITopicRepository
{
    Task<IEnumerable<TopicViewModel>> GetAllAsync();
}