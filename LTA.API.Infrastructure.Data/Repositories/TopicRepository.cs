using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly LtaApiContext _context;
    private IUserRepository UserRepository { get; }

    public TopicRepository(LtaApiContext context, IUserRepository userRepository)
    {
        _context = context;

        UserRepository = userRepository;
    }

    public IEnumerable<Topic> GetAll()
        => _context.Topics.Include(t => t.Categories);

    public async Task UpdateAsync(int id, int userId, bool isUserBeingAdded)
    {
        var topicToUpdate = await _context.Topics.FindAsync(id) ??
                            throw new NullReferenceException($"Cannot find the a topic with id: {id}");

        var user = await UserRepository.GetAsync(userId) ??
                        throw new NullReferenceException($"Cannot find the user with id: {userId}");

        if (isUserBeingAdded)
        {
            _context.Attach(topicToUpdate).State = EntityState.Modified;
            topicToUpdate.LastEntryDate = DateTime.Now;
            topicToUpdate.AddUser(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            if (!topicToUpdate.RemoveUser(user))
            {
                throw new InvalidOperationException(
                    $"Cannot remove user with id: {user.Id} from topic with id: {topicToUpdate.Id}");
            }
        }
    }

    public async Task<Topic> UpdateAndReturnAsync(int id, int userId, bool isUserBeingAdded)
    {
        await UpdateAsync(id, userId, isUserBeingAdded);

        return await _context.Topics.FindAsync(id) ??
               throw new NullReferenceException($"Cannot find topic with id: {id}");
    }

    public Topic? Get(int id) => _context.Topics.Find(id);


}