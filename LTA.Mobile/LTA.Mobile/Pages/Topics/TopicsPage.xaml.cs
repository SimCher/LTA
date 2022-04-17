using System.Windows.Input;
using Prism.Navigation;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Topics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopicsPage
    {

        private void SendReportCommandChanged(ICommand oldSendReportCommand, ICommand newSendReportCommand)
        { }
        public TopicsPage(INavigationService navService) : base(navService)
        {
            InitializeComponent();
        }
    }
}