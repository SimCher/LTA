using System.Threading.Tasks;

namespace LTA.Mobile.Client.ViewModels.Interfaces
{
    public interface IViewModel
    {
        Task Initialize();
        Task Stop();
    }
}