using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTA.Mobile.PageModels;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatRoomPage : ContentPage
    {
        private ChatRoomPageModel _vm;

        public ChatRoomPageModel ViewModel => _vm ??= (ChatRoomPageModel)BindingContext;

        public ChatRoomPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!DesignMode.IsDesignModeEnabled)
                ViewModel.ConnectCommand.Execute(null);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (!DesignMode.IsDesignModeEnabled)
                ViewModel.DisconnectCommand.Execute(null);
        }

        private async void ToolbarDone_Clicked(object sender, EventArgs e)
        {

        }

        //protected override bool OnBackButtonPressed()
        //{
        //    var vm = (ChatRoomPageModel)BindingContext;

        //    vm.ConfirmNavigateCommand.Execute(null);

        //    return base.OnBackButtonPressed();
        //}
    }
}