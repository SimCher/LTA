using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseTabbedPage
    {
        #region TabSelectedIndex
        public static readonly BindableProperty TabSelectedIndexProperty;

        public int TabSelectedIndex
        {
            get => (int)GetValue(TabSelectedIndexProperty);
            set => SetValue(TabSelectedIndexProperty, value);
        }
        private void TabSelectedIndexChanged(int oldTabSelectedIndex, int newTabSelectedIndex)
        { }
        #endregion

        #region ViewContent

        public static readonly BindableProperty ViewContentProperty;

        public View ViewContent
        {
            get => (View)GetValue(ViewContentProperty);
            set => SetValue(ViewContentProperty, value);
        }

        private void ViewContentChanged(View oldViewContent, View newViewContent)
        { }

        #endregion

        #region SettingsTappedCommand

        public static readonly BindableProperty SettingsTappedCommandProperty;

        public ICommand SettingsTappedCommand
        {
            get => (ICommand)GetValue(SettingsTappedCommandProperty);
            set => SetValue(SettingsTappedCommandProperty, value);
        }

        private void SettingsTappedCommandChanged(Type oldSettingsTappedCommand, Type newSettingsTappedCommand)
        { }

        #endregion

        #region TopicsTappedCommand

        public static readonly BindableProperty TopicsTappedCommandProperty;

        public ICommand TopicsTappedCommand
        {
            get => (ICommand)GetValue(TopicsTappedCommandProperty);
            set => SetValue(TopicsTappedCommandProperty, value);
        }

        private void TopicsTappedCommandChanged(ICommand oldTopicsTappedCommand, ICommand newTopicsTappedCommand)
        { }

        #endregion

        static BaseTabbedPage()
        {
            TabSelectedIndexProperty = BindableProperty.Create(
                nameof(TabSelectedIndex), typeof(int), typeof(BaseTabbedPage), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as BaseTabbedPage;
                    if (newV != null && newV is not int)
                        return;
                    var oldTabSelectedIndex = (int)old;
                    if (newV != null)
                    {
                        var newTabSelectedIndex = (int)newV;
                        me?.TabSelectedIndexChanged(oldTabSelectedIndex, newTabSelectedIndex);
                    }
                });

            ViewContentProperty = BindableProperty.Create(nameof(ViewContent),
                typeof(View), typeof(BaseTabbedPage), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as BaseTabbedPage;
                    if (newV != null && newV is not View) return;
                    var oldViewContent = (View)old;
                    var newViewContent = (View)newV;
                    me?.ViewContentChanged(oldViewContent, newViewContent);
                });

            SettingsTappedCommandProperty = BindableProperty.Create(nameof(SettingsTappedCommand), typeof(ICommand),
                typeof(BaseTabbedPage), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as BaseTabbedPage;
                    if (newV != null && !(newV is Type)) return;
                    var oldSettingsTappedCommand = (Type)old;
                    var newSettingsTappedCommand = (Type)newV;
                    me?.SettingsTappedCommandChanged(oldSettingsTappedCommand, newSettingsTappedCommand);
                });

            TopicsTappedCommandProperty = BindableProperty.Create(nameof(TopicsTappedCommand), typeof(ICommand),
                typeof(BaseTabbedPage), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as BaseTabbedPage;
                    if (newV != null && newV is not ICommand) return;
                    var oldMessagesTappedCommand = (ICommand)old;
                    var newMessagesTappedCommand = (ICommand)newV;
                    me?.TopicsTappedCommandChanged(oldMessagesTappedCommand, newMessagesTappedCommand);
                });
        }
        public BaseTabbedPage()
        {
            InitializeComponent();
            SettingsTappedCommand = new Command(() => Shell.Current.GoToAsync("///LetsTalkAbout/settings"));
            TopicsTappedCommand = new Command(() => Shell.Current.GoToAsync("///LetsTalkAbout/topics"));
        }
    }
}