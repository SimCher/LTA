// using System;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Linq;
// using System.Threading.Tasks;
// using LTA.Mobile.Application.Interfaces;
// using LTA.Mobile.Data.Context;
// using LTA.Mobile.Domain.Interfaces;
// using LTA.Mobile.Domain.Models;
// using Microsoft.EntityFrameworkCore;
// using Newtonsoft.Json;
//
// namespace LTA.Mobile.Data.Repositories;
//
// public class TopicRepository : ITopicRepository
// {
//     private readonly IChatService _chatService;
//
//     private LtaClientContext Context { get; }
//
//     public TopicRepository(LtaClientContext context, IChatService chatService)
//     {
//         Context = context;
//         _chatService = chatService;
//     }
//
//     public async Task<int> GetCountAsync()
//     {
//         return await _chatService.GetTopicsCountAsync();
//     }
//
//     public async Task<ICollection<Topic>> GetAll()
//     {
//         var t = Context.Topics.Count();
//         var allTopicsCount = await GetCountAsync();
//         if (allTopicsCount != Context.Topics.Count())
//         {
//             await InitializeAsync();
//         }
//
//         return Context.Topics.ToImmutableHashSet();
//     }
//
//     public async Task<Topic> GetAsync(int topicId)
//     {
//         return await Context.Topics.FindAsync(topicId.ToString());
//     }
//
//     public async Task AddUserInTopicAsync(User user, int topicId)
//     {
//         var topic = await GetAsync(topicId);
//
//         topic.UsersIn.Add(user);
//     }
//
//     public async Task AddAsync(Topic topic)
//     {
//         if (topic is null)
//         {
//             throw new NullReferenceException(nameof(topic));
//         }
//
//         await Context.Topics.AddAsync(topic);
//
//         await Context.SaveChangesAsync();
//     }
//
//     public async Task<bool> RemoveUserFromTopicAsync(string userCode, int topicId)
//     {
//         var topic = await GetAsync(topicId);
//
//         
//
//         return topic.UsersIn.Remove(topic.UsersIn.First(u => u.Code.Equals(userCode)));
//     }
//
//     private async Task InitializeAsync()
//     {
//         var topics = await GetTopicsAsync();
//         var definition = new
//         {
//             Id = 0,
//             UserId = 0,
//             Name = string.Empty,
//             Rating = .0f,
//             MaxUsersNumber = 0,
//             LastEntryDate = default(DateTime),
//             UserNumber = 0,
//             Categories = string.Empty
//         };
//
//         var enumerable = topics as object[] ?? topics.ToArray();
//         
//         if (!enumerable.Any())
//         {
//             return;
//         }
//         
//         var topicsObject = enumerable.Select(topic => 
//                 JsonConvert.DeserializeAnonymousType(topic.ToString(), definition))
//             .Select(anonTopic =>
//             {
//                 if (anonTopic == null) throw new NullReferenceException("Cannot deserialize the anonymous type");
//                 return new Topic
//                 {
//                     Id = anonTopic.Id,
//                     OwnerUserId = anonTopic.UserId,
//                     Name = anonTopic.Name,
//                     Rating = anonTopic.Rating,
//                     MaxUsersNumber = anonTopic.MaxUsersNumber,
//                     LastEntryDate = anonTopic.LastEntryDate,
//                     CurrentUsersNumber = anonTopic.UserNumber,
//                     CategoriesArray = anonTopic.Categories
//                 };
//             });
//
//         await Context.Topics.AddRangeAsync(topicsObject);
//
//         await Context.SaveChangesAsync();
//     }
//
//     private async Task<IEnumerable<object>> GetTopicsAsync()
//     {
//         if (!_chatService.IsConnected)
//         {
//             await _chatService.Connect();
//         }
//
//         IEnumerable<object> topics;
//
//         if (await Context.Topics.AnyAsync())
//         {
//             var ids = Context.Topics.Select(t => t.Id);
//             topics = new HashSet<object>(await _chatService.LoadTopicsAsync(ids));
//         }
//         else
//         {
//             topics = new HashSet<object>(await _chatService.LoadTopicsAsync());
//         }
//
//         return topics;
//     }
// }