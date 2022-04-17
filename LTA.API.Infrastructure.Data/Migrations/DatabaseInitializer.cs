using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;

namespace LTA.API.Infrastructure.Data.Migrations;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(LtaApiContext context)
    {
        await context.Database.EnsureCreatedAsync();


        if (context.Profiles.Any()) return;

        var newProfile = new Profile
        {
            Email = "dg743733@gmail.com",
            Phone = null,
            Password = "123456",
            Confirm = null,
            RegistrationDate = DateTime.Now
        };

        context.Profiles.Add(newProfile);

        var user = new User
        {
            Code = Guid.NewGuid().ToString("D"),
            LastEntryDate = DateTime.Now,
            Profile = newProfile
        };

        context.Users.Add(user);

        context.Chatters.Add(new Chatter
        {
            User = user
        });

        await context.SaveChangesAsync();

        var categories = new Category[] { new() { Name = "Cars" }, new() { Name = "Sport" } };
        context.Categories.AddRange(categories);

        var topics = new List<Topic>()
        {
            new()
            {
                Name = "Ferrari cars",
                MaxUsersNumber = 2,
                LastEntryDate = DateTime.Now,
                UserId = 1,
                Categories = new List<Category>() {categories[0]}
            },
            new()
            {
                Name = "FIFA World Cup 2022???",
                MaxUsersNumber = 16,
                LastEntryDate = DateTime.Now - TimeSpan.FromDays(144),
                UserId = 1,
                Categories = new List<Category>() {categories[1]}
            }
        };


        context.Topics.AddRange(topics);

        await context.SaveChangesAsync();
    }
}