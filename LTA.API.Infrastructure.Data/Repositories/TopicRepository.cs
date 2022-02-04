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
}