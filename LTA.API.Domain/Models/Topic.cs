using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LTA.API.Domain.Models;

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Rating { get; set; }
    public int MaxUsersNumber { get; set; }
    public bool IsBanned { get; set; }
    public DateTime LastEntryDate { get; set; }
    public int UserId { get; set; }

    public User? User { get; set; }
    public ICollection<Report> Reports { get; set; }
    public ICollection<Category> Categories { get; set; }

    [NotMapped] public int UserCount { get; set; }
    [NotMapped] public bool IsMultiuser => MaxUsersNumber > 2;

    public string GetCategoryNames()
    {
        var builder = new StringBuilder();

        foreach (var c in Categories)
        {
            builder.Append($"{c.Name},");
        }

        builder.Remove(builder.Length - 1, 1);

        return builder.ToString();
    }
}