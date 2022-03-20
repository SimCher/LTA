using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly LtaApiContext _context;
    private IUserRepository UserRepository { get; }
    private ICategoryRepository CategoryRepository { get; }

    public TopicRepository(LtaApiContext context, IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _context = context;

        UserRepository = userRepository;
        CategoryRepository = categoryRepository;
    }

    public IEnumerable<Topic> GetAll()
        => _context.Topics.Include(t => t.Categories);

    public async Task<Topic> AddAsync(string name, int maxUsers, string[] categories, string code)
    {
        var categoriesObject = await CategoryRepository.GetAll(categories);
        var userId = await UserRepository.GetIdAsync(code) ?? 1;
        var newTopic = new Topic
        {
            Name = name,
            MaxUsersNumber = maxUsers,
            Categories = new List<Category>(),
            UserId = userId
        };

        if (categoriesObject.Count != 0)
        {
            if (categoriesObject.Count == 1)
            {
                newTopic.Categories.Add(categoriesObject.First());
            }
            else
            {
                foreach (var category in categoriesObject)
                {
                    newTopic.Categories.Add(category);
                }
            }
        }

        _context.Topics.Add(newTopic);

        await _context.SaveChangesAsync();

        return newTopic;
    }

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