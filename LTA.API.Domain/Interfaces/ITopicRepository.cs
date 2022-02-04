using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface ITopicRepository
{
    IEnumerable<Topic> GetAll();
}