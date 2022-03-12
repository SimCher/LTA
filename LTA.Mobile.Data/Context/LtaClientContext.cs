using System.IO;
using LTA.Mobile.Data.Interfaces;
using LTA.Mobile.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

namespace LTA.Mobile.Data.Context;

public class LtaClientContext : DbContext, IDbContextable
{
    public DbSet<Message> Messages { get; set; }

    public LtaClientContext()
    {
        SQLitePCL.Batteries_V2.Init();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "lta.db3");

        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
}