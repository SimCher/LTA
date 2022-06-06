using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xamarin.Forms;

namespace LTA.API.Domain.Models;

public class Topic
{
    private int _userNumber;
    public int Id { get; set; }
    public string Name { get; set; }
    public float Rating { get; set; }
    public int MaxUsersNumber { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsForum { get; set; }
    
    public string InviteCode { get; set; }

    [NotMapped]
    public int UserNumber => Chatters.Count;

    public bool IsBanned { get; set; }
    public DateTime LastEntryDate { get; set; }
    public int UserId { get; set; }

    public User? User { get; set; }
    public ICollection<Report>? Reports { get; set; }
    public ICollection<Category>? Categories { get; set; }

    public ICollection<Chatter>? Chatters { get; set; }
    [NotMapped] public bool IsMultiuser => MaxUsersNumber > 2;

    public Topic()
    {
        Chatters ??= new List<Chatter>();
    }

    public bool UserContains(Chatter chatter)
    {
        return Chatters.Contains(chatter);
    }

    public void AddUser(Chatter chatter)
    {
        if (UserContains(chatter))
        {
            throw new InvalidOperationException($"User with id: {chatter.Id} is already in topic with id: {Id}.");
        }
        
        Chatters.Add(chatter);
    }

    //public bool RemoveUser(User user)
    //{
    //    if (!UserContains(user))
    //    {
    //        return false;
    //    }

    //    user.Color = default;
    //    ReleaseColor(user.Color);
    //    UsersIn.Remove(user);
    //    UserNumber = UsersIn.Count;
    //    return true;
    //}

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

    public Topic GetDeepCopy()
    {
        var other = (Topic)MemberwiseClone();
        
        other.Categories = new List<Category>(Categories);
        other.Chatters = new List<Chatter>(Chatters);
        other.Name = new StringBuilder(Name).ToString();
        other.Reports = new List<Report>(Reports);
        other.User = new User(User);

        return other;
    }
}