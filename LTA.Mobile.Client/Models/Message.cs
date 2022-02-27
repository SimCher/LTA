using LTA.Mobile.Client.Models.BaseModels;

namespace LTA.Mobile.Client.Models
{
    public class Message : BaseModel
    {
        public string Content { get; set; }
        public string TopicId { get; set; }
        public bool IsSent { get; set; }

        public bool IsSentPreviousMessage { get; set; }
        public Message ReplyTo { get; set; }
        public string SenderId { get; set; }
        public User Sender { get; set; }
    }
}