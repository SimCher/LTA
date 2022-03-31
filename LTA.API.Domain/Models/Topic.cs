using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xamarin.Forms;

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
    public ICollection<Report>? Reports { get; set; }
    public ICollection<Category>? Categories { get; set; }

    private Dictionary<Color, bool> AvailableColors { get; }


    [NotMapped] public ICollection<User> UsersIn { get; set; }
    [NotMapped] public bool IsMultiuser => MaxUsersNumber > 2;

    public Topic()
    {
        UsersIn = new HashSet<User>();

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

    public bool UserContains(User user)
    {
        return UsersIn.Contains(user);
    }

    public void AddUser(User user)
    {
        if (UserContains(user))
        {
            throw new InvalidOperationException($"User with id: {user.Id} is already in topic with id: {Id}.");
        }

        user.Color = GetAvailableColor();
        UsersIn.Add(user);
        UserNumber = UsersIn.Count;
    }

    public bool RemoveUser(User user)
    {
        if (!UserContains(user))
        {
            return false;
        }

        user.Color = default;
        ReleaseColor(user.Color);
        UsersIn.Remove(user);
        UserNumber = UsersIn.Count;
        return true;
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

    public Dictionary<string, Color> GetUsersCodeAndColor()
    {
        var usersCodeAndColor = new Dictionary<string, Color>();

        foreach (var user in UsersIn)
        {
            usersCodeAndColor[user.Code] = user.Color;
        }

        return usersCodeAndColor;
    }

    public void ReleaseColor(Color color)
    {
        AvailableColors[color] = true;
    }
}