using LTA.Mobile.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LTA.Mobile.Data.Interfaces;

public interface IDbContextable
{
    DbSet<Message> Messages { get; set; }
}