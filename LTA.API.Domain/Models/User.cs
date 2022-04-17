using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xamarin.Forms;

namespace LTA.API.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public bool IsAuth { get; set; }
    public DateTime LastEntryDate { get; set; }

    public Profile Profile { get; set; }

    public Chatter? Chatter { get; set; }

    public ICollection<Topic>? Topics { get; set; }

    public User() { }
    public User(User user)
    {
        Code = new StringBuilder(user.Code).ToString();
        Profile = user.Profile;
        Chatter = user.Chatter;
        Topics = new List<Topic>(Topics);
    }
}