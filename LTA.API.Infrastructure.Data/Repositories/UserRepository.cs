using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LtaApiContext _context;

    public UserRepository(LtaApiContext context)
    {
        _context = context;
    }

    public ValueTask<User?> GetAsync(int id)
    {
        return _context.Users.FindAsync(id);
    }

    public User Get(string userCode)
    {
        return _context.Users.First(u => u.Code != null && u.Code.Equals(userCode));
    }

    public int GetIdAsync(string code)
    {
        var user = Get(code);

        return user.Id;
    }

    public async Task CreateAsync(Profile profile)
    {
        var user = new User
        {
            Code = Guid.NewGuid().ToString("D"),
            IsAuth = true,
            LastEntryDate = DateTime.Now,
            Profile = profile
        };

        var chatter = new Chatter
        {
            User = user
        };

        _context.Chatters.Add(chatter);
        _context.Users.Add(user);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id)
    {
        var userToUpdate = await GetAsync(id);
        
        if (userToUpdate != null)
        {
        _context.Attach(userToUpdate).State = EntityState.Modified;

        
            userToUpdate.Code = Guid.NewGuid().ToString("D");
            userToUpdate.IsAuth = true;
            userToUpdate.LastEntryDate = DateTime.Now;

            userToUpdate.Chatter ??= _context.Chatters.First(c => c.Id == userToUpdate.Id);
            userToUpdate.Chatter.User = userToUpdate;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<User> UpdateAndReturnAsync(int id)
    {
        await UpdateAsync(id);

        return await _context.Users.FindAsync(id) ??
               throw new NullReferenceException($"Cannon find a user with id: {id}");
    }

    public async Task<bool> TryDeleteAsync(int id)
    {
        var userToDelete = await GetAsync(id);

        if (userToDelete == null)
        {
            return false;
        }

        userToDelete.Chatter ??= _context.Chatters.First(c => c.Id == userToDelete.Id);

        _context.Chatters.Remove(userToDelete.Chatter);
        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TryDeleteAsync(string userCode)
    {
        var userToDelete = await _context.Users.FirstAsync(u => u.Code == userCode);

        _context.Chatters.Remove(userToDelete.Chatter);
        _context.Users.Remove(userToDelete);

        await _context.SaveChangesAsync();

        return true;
    }
}