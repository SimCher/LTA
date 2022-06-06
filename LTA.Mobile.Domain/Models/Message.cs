#nullable enable
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using LTA.Mobile.Domain.Models.BaseModels;
using Xamarin.Forms;

namespace LTA.Mobile.Domain.Models
{
    public class Message : BaseModel
    {
        private string? _content;
        private byte[]? _image;
        private ImageSource? _picture;
        private string _userCode;
        private int _topicId;
        private bool _isSent;
        private DateTime _sentAt;
        private Message? _replyTo;

        public Topic Topic { get; set; }

        [NotMapped] public User Sender { get; set; }

        public string UserCode
        {
            get => _userCode;
            set => SetProperty(ref _userCode, value);
        }

        public bool IsSent
        {
            get => _isSent;
            set => SetProperty(ref _isSent, value);
        }

        public string? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public DateTime SentAt
        {
            get => _sentAt;
            set => SetProperty(ref _sentAt, value);
        }

        public byte[]? Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        [NotMapped] public string Time => CreationDate.ToString("HH:mm");

        public int TopicId
        {
            get => _topicId;
            set => _topicId = value > 0 ? value : throw new ArgumentException($"_topicId was less or equal 0!");
        }

        public bool IsSentPreviousMessage { get; set; }

        public Message? ReplyTo
        {
            get => _replyTo;
            set => SetProperty(ref _replyTo, value);
        }

        public override string ToString()
            => $"Id: {Id.ToString()}\n" +
               $"Creation date: {CreationDate.ToString(CultureInfo.CurrentCulture)}\n" +
               $"Content: {Content}\n" +
               $"User code: {UserCode}\n" +
               $"Sent at: {SentAt.ToString(CultureInfo.CurrentCulture)}\n" +
               $"Image: {Image?.ToString()}\n" +
               $"Topic id: {TopicId.ToString()}\n" +
               $"Reply to: {ReplyTo?.ToString()}\n";
    }
}