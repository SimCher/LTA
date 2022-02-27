using System.Threading.Tasks;
using LTA.Mobile.Client.Models;
using LTA.Mobile.Client.Services.Interfaces;

namespace LTA.Mobile.Client.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(IDataStore<User> userDataStore, ITopicDataStore topicDataStore, IMessageDataStore messageDataStore) : base(userDataStore, topicDataStore, messageDataStore)
        {
        }

        public override Task Initialize()
        {
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            return Task.CompletedTask;
        }
    }
}