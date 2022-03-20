using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages.Topics
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Add
    {
        public static readonly BindableProperty BackCommandProperty;

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }
        public Add()
        {
            InitializeComponent();

            BackCommand = new Command(async _ =>
                await Navigation.PopModalAsync());
        }

        static Add()
        {
            BackCommandProperty = BindableProperty.Create(nameof(BackCommand),
                typeof(ICommand), typeof(Add), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as Add;
                    if (newV != null && newV is not ICommand) return;
                    var oldBackCommand = (ICommand)old;
                    var newBackCommand = (ICommand)newV;
                    me?.BackCommandChanged(oldBackCommand, newBackCommand);
                });
        }

        private void BackCommandChanged(ICommand oldBackCommand, ICommand newBackCommand)
        { }
    }
}