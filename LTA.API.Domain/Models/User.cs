namespace LTA.API.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Code { get; set; }
    public bool IsAuth { get; set; }
    public DateTime LastEntryDate { get; set; }

    public Profile Profile { get; set; }
    
    public ICollection<Topic> Topics { get; set; }
}