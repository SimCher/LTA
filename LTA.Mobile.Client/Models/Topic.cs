using LTA.Mobile.Client.Models.BaseModels;

namespace LTA.Mobile.Client.Models
{
    public class Topic : BaseModel
    {
        public string[] UserIds { get; set; }
        private Message _lastMessage;

        public Message LastMessage
        {
            get => _lastMessage;
            set => SetProperty(ref _lastMessage, value);
        }

        private User _peer;

        public User Peer
        {
            get => _peer;
            set => SetProperty(ref _peer, value);
        }

        public Topic()
        {
            UserIds = new string[2];
        }
    }
}