using LTA.API.Domain.Models;

namespace LTA.API.Domain.Interfaces;

public interface IChatterRepository
{
    public Task<Chatter?> GetAsync(int id);
    public Task AddAsync(User user);

    public bool IsChatterInTopic(Topic topic);
}