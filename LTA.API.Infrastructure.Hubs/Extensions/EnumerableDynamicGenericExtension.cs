using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Hubs.Services;

namespace LTA.API.Infrastructure.Hubs.Extensions;

public static class CollectopnDynamicGenericExtension
{
    public static IEnumerable<dynamic> ToDynamicEnumerable(this IEnumerable<Topic> topicList) =>
        topicList.Select(GetNewDynamicTopic);

    public static IEnumerable<object> ToObjectEnumerable(this IEnumerable<Topic> topicList)
    {
        var topics = topicList.Select(GetNewObjectTopic);

        return topics;
    }


    private static object GetNewObjectTopic(Topic topic)
    {
        var objectTopic = new
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

        return objectTopic;
    }

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