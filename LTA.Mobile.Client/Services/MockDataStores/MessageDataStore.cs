using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Services.Interfaces;

namespace LTA.Mobile.Client.Services.MockDataStores
{
    public class MessageDataStore : IMessageDataStore
    {
        private readonly List<Message> _messages;

        public MessageDataStore(Topic topic)
        {
            _messages = new List<Message>();

            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                Content = "I was at your office yesterday.",
                CreationDate = DateTime.Now - TimeSpan.FromDays(1),
                IsSent = false,
                SenderId = topic.Peer.Id,
                Sender = topic.Peer
            });
            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                Content = "Ooh really ?",
                CreationDate = DateTime.Now - TimeSpan.FromDays(1),
                IsSent = true,
                SenderId = topic.UserIds[0],
            });
            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                IsSentPreviousMessage = true,
                Content = "Yeah. But you were not arround",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(5),
                IsSent = false,
                SenderId = topic.Peer.Id,
                Sender = topic.Peer
            });
            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                IsSentPreviousMessage = false,
                Content = "Yeah I was not arround I left early yesterday",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(2),
                IsSent = true,
                SenderId = topic.Peer.Id,
                Sender = topic.Peer,
                ReplyTo = _messages[_messages.Count - 3]
            });
            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                IsSentPreviousMessage = true,
                Content = "I sent this message",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(1),
                IsSent = true,
                SenderId = topic.Peer.Id,
                Sender = topic.Peer,
                ReplyTo = _messages[_messages.Count - 2]
            });
            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                IsSentPreviousMessage = true,
                Content = "I called you, and I left you a message did you see it ?",
                CreationDate = DateTime.Now - TimeSpan.FromMinutes(1),
                IsSent = false,
                SenderId = topic.Peer.Id,
                Sender = topic.Peer,
                ReplyTo = _messages[_messages.Count - 2]
            });
            _messages.Add(new Message
            {
                TopicId = topic.Id,
                Id = Guid.NewGuid().ToString(),
                IsSentPreviousMessage = false,
                Content = "I called you, and I left you a message did you see it ?",
                CreationDate = DateTime.Now,
                IsSent = false,
                SenderId = topic.Peer.Id,
                Sender = topic.Peer,
                ReplyTo = _messages[_messages.Count - 2]
            });

            topic.LastMessage = _messages.Last();
        }
        public Task<bool> AddItemAsync(Message item)
        {
            item.Id = Guid.NewGuid().ToString();
            _messages.Add(item);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateItemAsync(Message item)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Message> GetItemAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessagesForTopic(string topicId)
        {
            return Task.FromResult(_messages.Where(m => m.TopicId == topicId));
        }
    }
}