using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LTA.API.Domain.Models;

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Rating { get; set; }
    public int MaxUsersNumber { get; set; }
    public int UserNumber { get; set; }
    public bool IsBanned { get; set; }
    public DateTime LastEntryDate { get; set; }
    public int UserId { get; set; }

    public User? User { get; set; }
    public ICollection<Report> Reports { get; set; }
    public ICollection<Category> Categories { get; set; }


    [NotMapped] public ICollection<string> UsersIn { get; set; }
    [NotMapped] public bool IsMultiuser => MaxUsersNumber > 2;

    public Topic()
    {
        UsersIn = new HashSet<string>();
    }

    public bool ContainsUser(string userCode) => UsersIn.Contains(userCode);

    public bool TryAddUser(string userCode)
    {
        if (string.IsNullOrEmpty(userCode)) return false;
        UsersIn.Add(userCode);
        return true;

    }

    public bool TryRemoveUser(string userCode)
    {
        if (string.IsNullOrEmpty(userCode) || !ContainsUser(userCode))
            return false;
        return UsersIn.Remove(userCode);
    }

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