using System.IO;
using LTA.Mobile.Data.Interfaces;
using LTA.Mobile.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

namespace LTA.Mobile.Data.Context;

public sealed class LtaClientContext : DbContext, IDbContextable
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<Topic> Topics { get; set; }

    public LtaClientContext()
    {
        SQLitePCL.Batteries_V2.Init();
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "lta.db3");

        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Topic>().ToTable("topics").Property(t => t.Id).
            ValueGeneratedNever();

        modelBuilder.Entity<Message>().ToTable("messages")
            .HasOne(m => m.Topic)
            .WithMany(t => t.Messages);
    }
}