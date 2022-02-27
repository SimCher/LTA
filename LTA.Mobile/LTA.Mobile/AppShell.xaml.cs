using LTA.Mobile.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();
            SetTabBarIsVisible(this, false);
            //Routing.RegisterRoute("TopicsPage", typeof(TopicsPage));
        }
    }
}