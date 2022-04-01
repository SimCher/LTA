using LTA.Mobile.Pages.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LTA.Mobile.Pages.Topics;

namespace LTA.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();
            SetTabBarIsVisible(this, false);
            Routing.RegisterRoute("TopicsPage", typeof(TopicsPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
        }
    }
}