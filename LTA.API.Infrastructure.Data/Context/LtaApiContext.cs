using LTA.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Context;

public class LtaApiContext : DbContext
{
#pragma warning disable CS8618
    public LtaApiContext(DbContextOptions<LtaApiContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>().ToTable("Profiles")
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<User>(u => u.Id);

        modelBuilder.Entity<Topic>()
            .HasMany(t => t.Categories)
            .WithMany(c => c.Topics);
    }
}
#pragma warning restore CS8618