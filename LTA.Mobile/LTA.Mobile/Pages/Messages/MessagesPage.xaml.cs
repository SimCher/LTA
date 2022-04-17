using System.Windows.Input;
using LTA.Mobile.EventHandlers;
using LTA.Mobile.Helpers;
using LTA.Mobile.PageModels;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Messages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagesPage
    {
        #region BackCommand

        public static readonly BindableProperty BackCommandProperty;
        private MessagesPageModel ViewModel { get; }

        private INavigationService NavigationService { get; }

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        private void BackCommandChanged(ICommand oldBackCommand, ICommand newBackCommand)
        { }

        #endregion
        public MessagesPage(INavigationService navigationService)
        {
            InitializeComponent();

            NavigationService = navigationService;

            ViewModel = (MessagesPageModel)this.BindingContext;

            BackCommand = new Command(async _ =>
            {
                await Navigation.PopModalAsync();
            });
        }

        static MessagesPage()
        {
            BackCommandProperty = BindableProperty.Create(nameof(BackCommand),
                typeof(ICommand), typeof(MessagesPage), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as MessagesPage;
                    if (newV != null && newV is not ICommand)
                        return;
                    var oldBackCommand = (ICommand)old;
                    var newBackCommand = (ICommand)newV;
                    me?.BackCommandChanged(oldBackCommand, newBackCommand);
                });
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<BasePageModel, LTAFocusEventArgs>(this, "ShowKeyboard", (s, args) =>
                IsFocusOnKeyBoardChanged(args.IsFocused));

            MessagingCenter.Subscribe<BasePageModel, ScrollToItemEventArgs>(this, "ScrollToItem",
                (s, eargs) =>
                {
                    MessagesCollectionView.ScrollTo(eargs.Item);
                });
            base.OnAppearing();
        }

        protected override async void OnDisappearing()
        {
            await ViewModel.ChatService.LogOutFromChatAsync(ViewModel.CurrentTopic.Id);
            MessagingCenter.Unsubscribe<BasePageModel, LTAFocusEventArgs>(this, "ShowKeyboard");
            MessagingCenter.Unsubscribe<BasePageModel, ScrollToItemEventArgs>(this, "ScrollToItem");
            base.OnDisappearing();
        }

        private void IsFocusOnKeyBoardChanged(bool newIsFocusOnKeyBoard)
        {
            if (newIsFocusOnKeyBoard)
                TextInput.Focus();
            else TextInput.Unfocus();
        }

        private void TextInput_Focused(object sender, LTAFocusEventArgs e)
        {
            if (!e.IsFocused)
                VisualStateManager.GoToState(TextInput, "Unfocused");
        }
    }
}