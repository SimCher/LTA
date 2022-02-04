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

    public async Task<User?> GetAsync(int id)
        => await _context.Users.FindAsync(id);
        

    public async Task CreateAsync(Profile profile)
    {
        _context.Users.Add(new User
        {
            Code = Guid.NewGuid().ToString("D"),
            IsAuth = true,
            LastEntryDate = DateTime.Now,
            Profile = profile
        });

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id)
    {
        var userToUpdate = await _context.Users.FindAsync(id) ??
                           throw new NullReferenceException($"Cannot find the user with id {id}");

        _context.Attach(userToUpdate).State = EntityState.Modified;

        userToUpdate.Code = Guid.NewGuid().ToString("D");
        userToUpdate.IsAuth = true;
        userToUpdate.LastEntryDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task<User> UpdateAndReturnAsync(int id)
    {
        await UpdateAsync(id);

        return await _context.Users.FindAsync(id) ??
               throw new NullReferenceException($"Cannot find the user with id {id}");
    }

    public async Task<bool> TryDeleteAsync(int id)
    {
        var userToDelete = await _context.Users.FindAsync(id);

        if (userToDelete == null)
        {
            return false;
        }

        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TryDeleteAsync(string userCode)
    {
        var userToDelete = await _context.Users.FirstAsync(u => u.Code == userCode);

        _context.Users.Remove(userToDelete);

        await _context.SaveChangesAsync();

        return true;
    }
}