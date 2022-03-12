using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopicsPage
    {
        public TopicsPage(INavigationService navService)
        {
            InitializeComponent();
        }
    }
}