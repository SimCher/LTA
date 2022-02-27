using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Services.Interfaces;

namespace LTA.Mobile.Client.Services.MockDataStores
{
    public class TopicDataStore : ITopicDataStore
    {
        private readonly List<Topic> _topics;

        public TopicDataStore(User currentUser, List<User> users)
        {
            var messages = new[]
            {
                "Hi, am ok and you?",
                "Hi, what's up?",
                "I was aware of that",
                "Get up.",
                "Hello how are you?"
            };

            _topics = new List<Topic>();

            foreach (var user in users)
            {
                int randomHours = new Random().Next(0, 24);
                _topics.Add(new Topic
                {
                    Id = Guid.NewGuid().ToString(),
                    LastMessage = new Message
                    {
                        Content = messages[new Random().Next(0, messages.Length - 1)],
                        IsSent = true,
                        CreationDate = DateTime.UtcNow - TimeSpan.FromHours(randomHours),
                        Sender = user
                    },
                    Peer = user,
                    UserIds = new string[] { currentUser.Id, user.Id }
                });
            }

            _topics.OrderByDescending(t => t.LastMessage.CreationDate);
        }
        public Task<bool> AddItemAsync(Topic item)
        {
            _topics.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateItemAsync(Topic item)
        {
            var topic = _topics.FirstOrDefault(c => c.Id == item.Id);
            var index = _topics.IndexOf(topic);
            _topics[index] = topic;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            var topic = GetItemAsync(id);
            if (topic == null)
                return Task.FromResult(false);
            _topics.Remove(topic.Result);
            return Task.FromResult(true);

        }

        public Task<Topic> GetItemAsync(string id)
        {
            return Task.FromResult(_topics.FirstOrDefault(t => t.Id == id));
        }

        public Task<IEnumerable<Topic>> GetItemsAsync(bool forceRefresh = false)
        {
            return Task.FromResult((IEnumerable<Topic>)_topics);
        }

        public async Task<IEnumerable<Topic>> GetTopicsForUser(string userId)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return await Task.FromResult(_topics.Where(t => t.UserIds[0] == userId));
        }
    }
}