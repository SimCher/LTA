using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Messages.Popups;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class InvitePopup : StackLayout
{
    public ICommand CloseCommand { get; }
    public InvitePopup()
    {
        InitializeComponent();
        
        CloseCommand = new Command(_ =>
        {

            Navigation.PopModalAsync();
        });
    }
}