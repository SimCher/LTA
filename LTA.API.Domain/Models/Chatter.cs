using System.ComponentModel.DataAnnotations.Schema;
using Xamarin.Forms;

namespace LTA.API.Domain.Models;

public class Chatter
{
    public int Id { get; set; }

    public int? TopicId { get; set; }

    [NotMapped]
    public Color Color { get; set; }

    public User? User { get; set; }

    public Topic? Topic { get; set; }
}