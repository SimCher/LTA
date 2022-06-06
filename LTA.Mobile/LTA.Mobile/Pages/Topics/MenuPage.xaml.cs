using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Topics;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MenuPage : ContentPage
{
    public MenuPage(INavigationService navService)
    {
        InitializeComponent();
    }
}