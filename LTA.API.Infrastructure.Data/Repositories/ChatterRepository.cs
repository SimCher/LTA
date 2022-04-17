using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;

namespace LTA.API.Infrastructure.Data.Repositories;

public class ChatterRepository : IChatterRepository
{
    private LtaApiContext Context { get; }

    public ChatterRepository(LtaApiContext context)
    {
        Context = context;
    }
    public async Task<Chatter?> GetAsync(int id)
    {
        return await Context.Chatters.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public bool IsChatterInTopic(Topic topic)
    {
        throw new NotImplementedException();
    }
}