namespace LTA.Mobile.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public bool IsOwner { get; set; }
    }
}