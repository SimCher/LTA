namespace LTA.API.Infrastructure.Hubs.Interfaces;

public interface ITopicService
{
    public IEnumerable<dynamic> GetTopicsDynamic();
}