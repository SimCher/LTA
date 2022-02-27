using LTA.API.Domain.Models;

namespace LTA.API.Infrastructure.Hubs.Extensions;

public static class CollectopnDynamicGenericExtension
{
    public static IEnumerable<dynamic> ToDynamicEnumerable(this IEnumerable<Topic> topicList) =>
        topicList.Select(topic => GetNewDynamicTopic(topic)).ToList();

    private static dynamic GetNewDynamicTopic(Topic topic)
    {
        dynamic dynamicTopic = new
        {
            topic.Id,
            topic.UserId,
            topic.Name,
            topic.Rating,
            topic.MaxUsersNumber,
            topic.LastEntryDate,
            topic.UserNumber,
            Categories = topic.GetCategoryNames()
        };

        return dynamicTopic;
    }
}