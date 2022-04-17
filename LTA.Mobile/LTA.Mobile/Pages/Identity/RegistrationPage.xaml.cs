using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Identity
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();

            var existingPages = Navigation.NavigationStack.ToList();

            foreach (var page in existingPages)
            {
                Navigation.RemovePage(page);
            }
        }
    }
}