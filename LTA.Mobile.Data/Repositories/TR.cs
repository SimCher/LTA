// using System;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Data.Common;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using LTA.Mobile.Application.Interfaces;
// using LTA.Mobile.Data.Context;
// using LTA.Mobile.Domain.Interfaces;
// using LTA.Mobile.Domain.Models;
// using Newtonsoft.Json;
//
// namespace LTA.Mobile.Data.Repositories;
//
// public class TopicRepository : ITopicRepository
// {
//     private LtaClientContext Context { get; }
//     private IChatService ChatService { get; }
//     
//     public TopicRepository(LtaClientContext context, IChatService chatService)
//     {
//         Context = context;
//         ChatService = chatService;
//     }
//     
//     public async Task<ICollection<Topic>> GetAll()
//     {
//         await InitializeAsync();
//         return Context.Topics.ToList();
//     }
//
//     public async Task<ICollection<Topic>> GetAllAsync()
//     {
//         throw new NotImplementedException();
//     }
//
//     public async Task<Topic> GetAsync(int topicId)
//     {
//         return await Context.Topics.FindAsync(topicId.ToString());
//     }
//
//     public async Task AddUserInTopicAsync(User user, int topicId)
//     {
//         throw new NotImplementedException();
//     }
//
//     public async Task AddAsync(Topic topic)
//     {
//         throw new NotImplementedException();
//     }
//
//     public async Task<int> GetCountAsync()
//     {
//         throw new NotImplementedException();
//     }
//
//     public async Task<bool> RemoveUserFromTopicAsync(string userCode, int topicId)
//     {
//         throw new NotImplementedException();
//     }
//
//     private async Task InitializeAsync()
//     {
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
//         if (ChatService.IsConnected)
//         {
//             var topics = (await ChatService.LoadTopicsAsync()).ToList();
//
//             if (!topics.Any())
//             {
//                 return;
//             }
//
//             var topicObjects = topics.Select(topic =>
//                     JsonConvert.DeserializeAnonymousType(topic.ToString(), definition))
//                 .Select(anonTopic => new Topic
//                 {
//                     Id = anonTopic.Id,
//                     OwnerUserId = anonTopic.UserId,
//                     Name = anonTopic.Name,
//                     Rating = anonTopic.Rating,
//                     MaxUsersNumber = anonTopic.MaxUsersNumber,
//                     LastEntryDate = anonTopic.LastEntryDate,
//                     CurrentUsersNumber = anonTopic.UserNumber,
//                     CategoriesArray = anonTopic.Categories
//                 });
//
//             await Task.WhenAll
//             (
//                 Context.Topics.AddRangeAsync(topicObjects),
//                 Context.SaveChangesAsync()
//             );
//         }
//         
//         
//         
//         
//     }
// }