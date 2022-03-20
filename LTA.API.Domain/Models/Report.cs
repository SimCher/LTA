namespace LTA.API.Domain.Models;

public class Report
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public int TopicId { get; set; }

    public Topic Topic { get; set; }
}