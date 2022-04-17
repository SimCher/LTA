using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly LtaApiContext _context;
    private IUserRepository UserRepository { get; }
    private IChatterRepository ChatterRepository { get; }
    private ICategoryRepository CategoryRepository { get; }

    public TopicRepository(LtaApiContext context, IUserRepository userRepository,
        ICategoryRepository categoryRepository, IChatterRepository chatterRepository)
    {
        _context = context;

        UserRepository = userRepository;
        ChatterRepository = chatterRepository;
        CategoryRepository = categoryRepository;
    }

    public IEnumerable<Topic> GetAll()
        => _context.Topics.Include(t => t.Categories)
            .Include(t => t.Chatters);

    public async Task<Topic> AddAsync(Topic topic)
    {
        _context.Topics.Add(topic);

        await _context.SaveChangesAsync();

        return topic;
    }

    public async Task<bool> AddChatterInTopic(int topicId, Chatter chatter)
    {
        var topic = Get(topicId) ??
                    throw new NullReferenceException($"topic with id: {topicId} was null");

        if (topic.Chatters != null && !topic.Chatters.Contains(chatter))
        {
            topic.Chatters.Add(chatter);
            await _context.SaveChangesAsync();

            return true;
        }

        return false;
    }

    public async Task<bool> RemoveChatterFromTopic(int topicId, Chatter chatter)
    {
        var topic = Get(topicId) ??
                    throw new NullReferenceException($"topic with id: {topicId} was null");

        if (topic.Chatters != null && topic.Chatters.Remove(chatter))
        {
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<Topic> UpdateAsync(int id, Topic topic)
    {
        var topicObject = Get(id) ?? throw new NullReferenceException("topicObject was null");
        _context.Attach(topicObject).State = EntityState.Modified;
        topicObject = topic.GetDeepCopy();
        await _context.SaveChangesAsync();

        return topicObject;
    }

    //public async Task<Topic> UpdateAsync(int id, string userCode, bool isUserBeingAdded)
    //{
    //    var topicToUpdate = await _context.Topics.FindAsync(id) ??
    //                        throw new NullReferenceException($"Cannot find the a topic with id: {id}");

    //    var chatter = (await UserRepository.GetAsync(userCode))?.Chatter ??
    //                    throw new NullReferenceException($"Cannot find the user with id: {userCode}");

    //    if (isUserBeingAdded)
    //    {
    //        _context.Attach(topicToUpdate).State = EntityState.Modified;
    //        topicToUpdate.LastEntryDate = DateTime.Now;
    //        topicToUpdate.AddUser(chatter);
    //        await _context.SaveChangesAsync();
    //    }
    //    else
    //    {
    //        if (!topicToUpdate.RemoveUser(user))
    //        {
    //            throw new InvalidOperationException(
    //                $"Cannot remove user with id: {user.Id} from topic with id: {topicToUpdate.Id}");
    //        }
    //    }

    //    return topicToUpdate;
    //}

    //public async Task<Topic> UpdateAndReturnAsync(int id, string userCode, bool isUserBeingAdded)
    //{
    //    return await UpdateAsync(id, userCode, isUserBeingAdded);
    //}

    public Topic? Get(int id) => _context.Topics.Find(id);


}