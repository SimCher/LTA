using System.Threading.Tasks;

namespace LTA.Mobile.Client.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoToPage(string route);
    }
}