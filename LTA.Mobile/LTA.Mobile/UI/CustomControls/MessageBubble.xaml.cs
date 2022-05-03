using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.UI.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageBubble
    {
        #region Text

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(MessageBubble), propertyChanged: (obj, old, newV) =>
            {
                var me = obj as MessageBubble;
                if (newV != null && newV is not string) return;
                var oldText = (string)old;
                var newText = (string)newV;
                me?.TextChanged(oldText, newText);
            });

        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image),
            typeof(ImageSource),
            typeof(MessageBubble), propertyChanged: (obj, old, newV) =>
            {
                var me = obj as MessageBubble;
                if (newV != null && newV is not ImageSource) return;
                var oldImage = (ImageSource) old;
                var newImage = (ImageSource) newV;
                me?.ImageChanged(oldImage, newImage);
            });

        private void TextChanged(string oldText, string newText)
        {

        }

        private void ImageChanged(ImageSource oldImage, ImageSource newImage) {}

        /// <summary>
        /// A bindable property
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageSource Image
        {
            get => (ImageSource) GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
        #endregion

        #region TextColor
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MessageBubble), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as MessageBubble;
            if (newV != null && !(newV is Color)) return;
            var oldBackgroundColor = (Color)old;
            if (newV != null)
            {
                var newBackgroundColor = (Color)newV;
                me?.BackgroundColorChanged(oldBackgroundColor, newBackgroundColor);
            }
        });

        private void BackgroundColorChanged(Color oldBackgroundColor, Color newBackgroundColor)
        {

        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
        #endregion



        #region ReplyTappedCommand

        public static readonly BindableProperty ReplyTappedCommandProperty = BindableProperty.Create(
            nameof(ReplyTappedCommand), typeof(ICommand), typeof(MessageBubble), propertyChanged: (obj, old, newV) =>
            {
                var me = obj as MessageBubble;
                if (newV != null && !(newV is ICommand)) return;
                var oldReplyTappedCommand = (ICommand)old;
                var newReplyTappedCommand = (ICommand)newV;
                me?.ReplyTappedCommandChanged(oldReplyTappedCommand, newReplyTappedCommand);
            });

        private void ReplyTappedCommandChanged(ICommand oldReplyTappedCommand, ICommand newReplyTappedCommand)
        {

        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public ICommand ReplyTappedCommand
        {
            get => (ICommand)GetValue(ReplyTappedCommandProperty);
            set => SetValue(ReplyTappedCommandProperty, value);
        }
        #endregion

        #region ReplyBackgroundOpacity

        public static readonly BindableProperty ReplyBackgroundOpacityProperty = BindableProperty.Create(
            nameof(ReplyBackgroundOpacity),
            typeof(float), typeof(MessageBubble), defaultValue: 1f, propertyChanged: (obj, old, newV) =>
            {
                var me = obj as MessageBubble;
                if (newV != null && !(newV is Type)) return;
                var oldReplyBackgroundOpacity = (Type)old;
                var newReplyBackgroundOpacity = (Type)newV;
                me?.ReplyBackgroundOpacityChanged(oldReplyBackgroundOpacity, newReplyBackgroundOpacity);
            });

        private void ReplyBackgroundOpacityChanged(Type oldReplyBackgroundOpacity, Type newReplyBackgroundOpacity)
        {
        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public Type ReplyBackgroundOpacity
        {
            get => (Type)GetValue(ReplyBackgroundOpacityProperty);
            set => SetValue(ReplyBackgroundOpacityProperty, value);
        }
        #endregion



        public MessageBubble()
        {
            InitializeComponent();
        }
    }
}