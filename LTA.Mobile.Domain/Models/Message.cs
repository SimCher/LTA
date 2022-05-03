using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using LTA.Mobile.Domain.Models.BaseModels;
using Xamarin.Forms;

namespace LTA.Mobile.Domain.Models
{
    public class Message : BaseModel
    {
        private string _content;
        private byte[] _image;
        private ImageSource _picture;
        private string _userCode;
        private int _topicId;
        private bool _isOwner;
        private bool _isSent;
        private DateTime _sentAt;
        private Message _replyTo;

        public Topic Topic { get; set; }

        [NotMapped]
        public User Sender { get; set; }

        public string UserCode
        {
            get => _userCode;
            set => SetProperty(ref _userCode, value);
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

        public byte[] Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        [NotMapped]
        public string Time => CreationDate.ToString("HH:mm");

        public int TopicId
        {
            get => _topicId;
            set => _topicId = value > 0 ? value : throw new ArgumentException($"_topicId was less or equal 0!");
        }

        public bool IsSentPreviousMessage { get; set; }

        public Message ReplyTo
        {
            get => _replyTo;
            set => SetProperty(ref _replyTo, value);
        }

        public Message()
        {
            
        }
    }
}