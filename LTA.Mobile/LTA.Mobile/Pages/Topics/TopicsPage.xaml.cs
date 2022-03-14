using Prism.Navigation;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Topics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopicsPage
    {
        public TopicsPage(INavigationService navService) : base(navService)
        {
            InitializeComponent();
        }
    }
}