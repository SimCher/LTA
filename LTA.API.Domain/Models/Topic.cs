using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ReactiveUI;
using Xamarin.Forms;

namespace LTA.API.Domain.Models;

public class Topic
{
    private int _userNumber;
    public int Id { get; set; }
    public string Name { get; set; }
    public float Rating { get; set; }
    public int MaxUsersNumber { get; set; }

    [NotMapped]
    public int UserNumber => Chatters.Count;

    public bool IsBanned { get; set; }
    public DateTime LastEntryDate { get; set; }
    public int UserId { get; set; }

    public User? User { get; set; }
    public ICollection<Report>? Reports { get; set; }
    public ICollection<Category>? Categories { get; set; }

    private Dictionary<Color, bool> AvailableColors { get; set; }


    public ICollection<Chatter>? Chatters { get; set; }
    [NotMapped] public bool IsMultiuser => MaxUsersNumber > 2;

    public Topic()
    {
        Chatters ??= new List<Chatter>();

        AvailableColors = new Dictionary<Color, bool>
        {
            {Color.Blue, true},
            {Color.Red, true},
            {Color.Green, true},
            {Color.Purple, true},
            {Color.Black, true},
            {Color.Pink, true},
            {Color.Yellow, true},
            {Color.Orange, true},
            {Color.White, true},
            {Color.Brown, true},
            {Color.Cyan, true},
            {Color.Gray, true},
            {Color.Magenta, true},
            {Color.Salmon, true},
            {Color.Teal, true},
            {Color.Tomato, true}
        };
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

        chatter.Color = GetAvailableColor();
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

    public Color GetAvailableColor()
    {
        var availableColor = AvailableColors.First(ac => ac.Value).Key;
        AvailableColors[availableColor] = false;
        return availableColor;
    }

    public Color[] GetColors()
    {
        return AvailableColors.Where(ac => !ac.Value).Select(ac => ac.Key).ToArray();
    }

    public void ReleaseColor(Color color)
    {
        AvailableColors[color] = true;
    }

    public Topic GetDeepCopy()
    {
        var other = (Topic)MemberwiseClone();

        other.AvailableColors = new Dictionary<Color, bool>(AvailableColors);
        other.Categories = new List<Category>(Categories);
        other.Chatters = new List<Chatter>(Chatters);
        other.Name = new StringBuilder(Name).ToString();
        other.Reports = new List<Report>(Reports);
        other.User = new User(User);

        return other;
    }
}