using System;
using System.ComponentModel.DataAnnotations.Schema;
using LTA.Mobile.Domain.Models.BaseModels;
using Xamarin.Forms;

namespace LTA.Mobile.Domain.Models
{
    public class Message : BaseModel
    {
        [NotMapped]
        private static readonly Random Random;

        private string _content;
        private int _userId;
        private int _topicId;
        private bool _isOwner;
        private bool _isSent;
        private DateTime _sentAt;
        private Color _color;

        public Topic Topic { get; set; }

        public User Sender { get; set; }

        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public bool IsOwner
        {
            get => _isOwner;
            set => SetProperty(ref _isOwner, value);
        }

        public bool IsSent
        {
            get => _isSent;
            set => SetProperty(ref _isSent, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public DateTime SentAt
        {
            get => _sentAt;
            set => SetProperty(ref _sentAt, value);
        }

        public int TopicId
        {
            get => _topicId;
            set => _topicId = value > 0 ? value : throw new ArgumentException($"_topicId was less or equal 0!");
        }

        public bool IsSentPreviousMessage { get; set; }

        public Message ReplyTo { get; set; }
    }
}