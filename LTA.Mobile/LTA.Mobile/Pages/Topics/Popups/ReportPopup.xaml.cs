
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Topics.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPopup : StackLayout
    {
        public ICommand CloseCommand { get; }
        public ReportPopup()
        {
            InitializeComponent();

            CloseCommand = new Command(_ =>
            {

                Navigation.PopModalAsync();
            });
        }
    }
}