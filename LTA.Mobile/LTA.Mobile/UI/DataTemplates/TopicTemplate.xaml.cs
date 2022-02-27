using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.UI.DataTemplates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopicTemplate
    {
        public static readonly BindableProperty IsInTitleProperty;
        public static readonly BindableProperty IsUsersTypingProperty;
        public TopicTemplate()
        {
            InitializeComponent();
        }

        static TopicTemplate()
        {
            IsInTitleProperty = BindableProperty.Create(nameof(IsInTitle),
                typeof(bool), typeof(TopicTemplate), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as TopicTemplate;
                    if (newV != null && newV is not bool)
                        return;
                    var oldIsInTitle = (bool)old;
                    var newIsInTitle = newV != null && (bool)newV;
                    me?.IsInTitleChanged(oldIsInTitle, newIsInTitle);
                });

            IsUsersTypingProperty = BindableProperty.Create(nameof(IsUsersTyping),
                typeof(bool), typeof(TopicTemplate), propertyChanged: (obj, old, newV) =>
                {
                    var me = obj as TopicTemplate;
                    if (newV != null && newV is not bool)
                        return;
                    var oldIsTyping = (bool)old;
                    var newIsTyping = newV != null && (bool)newV;

                    me?.IsTypingChanged(oldIsTyping, newIsTyping);
                });
        }

        public bool IsInTitle
        {
            get => (bool)GetValue(IsInTitleProperty);
            set => SetValue(IsInTitleProperty, value);
        }

        public bool IsUsersTyping
        {
            get => (bool)GetValue(IsUsersTypingProperty);
            set => SetValue(IsUsersTypingProperty, value);
        }

        private void IsInTitleChanged(bool oldIsInTitle, bool newIsInTitle)
        { }

        private void IsTypingChanged(bool oldIsTyping, bool newIsTyping)
        { }


    }
}