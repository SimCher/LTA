using LTA.Mobile.Client.Models.BaseModels;

namespace LTA.Mobile.Client.Models
{
    public class User : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public int NumberOfConversations { get; set; }
        public int NumberOfMessagesSent { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsOnline { get; set; }

        public User()
        {

        }

        public User(string firstName, string secondName, string bio, string profilePicture, int numberOfConversations,
            int numberOfMessagesSent)
        {
            FirstName = firstName;
            LastName = secondName;
            NumberOfConversations = numberOfConversations;
            Bio = bio;
            ProfilePicture = profilePicture;
            NumberOfMessagesSent = numberOfMessagesSent;
        }
    }
}