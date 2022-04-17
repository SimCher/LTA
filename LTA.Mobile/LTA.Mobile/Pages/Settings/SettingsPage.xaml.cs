using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LTA.Mobile.Pages.Base;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : BaseTabbedPage
    {

        public SettingsPage(INavigationService navService) : base(navService)
        {
            InitializeComponent();
        }
    }
}
