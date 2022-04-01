using System.Windows.Input;
using LTA.Mobile.Pages.Messages;
using LTA.Mobile.Pages.Topics.Popups;
using Prism.Navigation;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
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