using LTA.API.Domain.Models;

namespace LTA.API.Infrastructure.Hubs.Extensions;

public static class CollectopnDynamicGenericExtension
{
    public static IEnumerable<dynamic> ToDynamicEnumerable(this IEnumerable<Topic> topicList) =>
        topicList.Select(GetNewDynamicTopic);

    public static IEnumerable<object> ToObjectEnumerable(this IEnumerable<Topic> topicList) =>
        topicList.Select(GetNewObjectTopic);

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
            UsersIn = topic.GetUsersCodeAndColor(),
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
            UsersIn = topic.GetUsersCodeAndColor(),
            Categories = topic.GetCategoryNames()
        };

        return dynamicTopic;
    }
}