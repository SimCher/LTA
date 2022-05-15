using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly LtaApiContext _context;

    public ProfileRepository(LtaApiContext context)
    {
        _context = context;
    }

    public IEnumerable<Profile> GetAll()
        => _context.Profiles;

    // public async Task<Profile?> GetAsync(string phoneOrEmail)
    //     => await _context.Profiles.SingleOrDefaultAsync(p =>
    //         p.Phone == phoneOrEmail || p.Email == phoneOrEmail);

    public Task<Profile?> GetAsync(string phoneOrEmail)
    {
        return _context.Profiles.SingleOrDefaultAsync( p =>
            p.Phone.Equals(phoneOrEmail) || p.Email.Equals(phoneOrEmail));
    }

    public async Task? CreateAsync(string? phone, string? email, string password, string? confirm)
    {
        if (phone == null && email == null)
        {
            throw new ArgumentException($"Both {nameof(phone)} and {nameof(email)} had value null");
        }

        _context.Profiles.Add(new Profile()
        {
            Phone = phone,
            Email = email,
            Password = password,
            Confirm = confirm,
            RegistrationDate = DateTime.Now
        });

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id)
    {
        var profileToUpdate = await _context.Profiles.FindAsync(id);

        if (profileToUpdate == null)
        {
            throw new NullReferenceException($"Cannot find the profile with id {id}");
        }

        _context.Attach(profileToUpdate).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }

    public Task<Profile?>? IsDataValidAsync(string phoneOrEmail, string password)
        => _context.Profiles.FirstOrDefaultAsync(p =>
            (p.Phone.Equals(phoneOrEmail) || p.Email.Equals(phoneOrEmail)) && p.Password.Equals(password));

    public async Task<bool> TryDeleteAsync(int id)
    {
        var profileToDelete = await _context.Profiles.FindAsync(id);

        if (profileToDelete == null)
        {
            return false;
        }

        _context.Profiles.Remove(profileToDelete);
        await _context.SaveChangesAsync();

        return true;
    }
}