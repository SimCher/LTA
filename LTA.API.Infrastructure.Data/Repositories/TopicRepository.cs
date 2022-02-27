using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly LtaApiContext _context;

    public TopicRepository(LtaApiContext context)
    {
        _context = context;
    }

    public IEnumerable<Topic> GetAll()
        => _context.Topics.Include(t => t.Categories);

    public async Task UpdateAsync(int id, int usersCount)
    {
        var topicToUpdate = await _context.Topics.FindAsync(id) ??
                            throw new NullReferenceException($"Cannot find the a topic with id: {id}");

        _context.Attach(topicToUpdate).State = EntityState.Modified;

        topicToUpdate.LastEntryDate = DateTime.Now;
        topicToUpdate.UserNumber = usersCount;

        await _context.SaveChangesAsync();
    }

    public async Task<Topic> UpdateAndReturnAsync(int id, int usersCount)
    {
        await UpdateAsync(id, usersCount);

        return await _context.Topics.FindAsync(id) ??
               throw new NullReferenceException($"Cannot find topic with id: {id}");
    }

    public Topic? Get(int id) => _context.Topics.Find(id);


}