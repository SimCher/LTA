namespace LTA.API.Domain.Models;

public class Profile
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Password { get; set; }
    public string? Confirm { get; set; }
    public bool IsBanned { get; set; }
    public DateTime RegistrationDate { get; set; }
    
    public User? User { get; set; }
}