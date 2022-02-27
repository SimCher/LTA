using System;
using System.Threading.Tasks;
using LTA.Mobile.Client.Services.Interfaces;
using Xamarin.Forms;

namespace LTA.Mobile.Client.Services.Navigation
{
    public class SimpleNavigationService : INavigationService
    {
        public Task GoToPage(string route)
        {
            try
            {
                return Shell.Current.GoToAsync(route);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}