using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.UI.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FloatingButton
    {
        #region PressedCommand

        public static readonly BindableProperty PressedCommandProperty = BindableProperty.Create(nameof(PressedCommand),
            typeof(ICommand), typeof(FloatingButton), propertyChanged: (obj, old, newV) =>
            {
                var me = obj as FloatingButton;
                if (newV != null && newV is not ICommand) return;
                var oldPressedCommand = (ICommand)old;
                var newPressedCommand = (ICommand)newV;
                me?.PressedCommandChanged(oldPressedCommand, newPressedCommand);
            });

        private void PressedCommandChanged(ICommand oldPressedCommand, ICommand newPressedCommand)
        {

        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public ICommand PressedCommand
        {
            get => (ICommand)GetValue(PressedCommandProperty);
            set => SetValue(PressedCommandProperty, value);
        }
        #endregion

        #region IconSize
        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(nameof(IconSize), typeof(float), typeof(FloatingButton), defaultValue: 20f, propertyChanged: (obj, old, newV) =>
        {
            var me = obj as FloatingButton;
            if (newV != null && newV is not float) return;
            var oldIconSize = (float)old;
            if (newV != null)
            {
                var newIconSize = (float)newV;
                me?.IconSizeChanged(oldIconSize, newIconSize);
            }
        });

        private void IconSizeChanged(float oldIconSize, float newIconSize)
        {
        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public float IconSize
        {
            get => (float)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        #endregion

        #region Icon
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(String), typeof(FloatingButton), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as FloatingButton;
            if (newV != null && newV is not string) return;
            var oldIcon = (string)old;
            var newIcon = (string)newV;
            me?.IconChanged(oldIcon, newIcon);
        });

        private void IconChanged(string oldIcon, string newIcon)
        {

        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public String Icon
        {
            get => (String)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        #endregion

        #region IconColor
        public static readonly BindableProperty IconColorProperty = BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(FloatingButton), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as FloatingButton;
            if (newV != null && newV is not Color) return;
            var oldIconColor = (Color)old;
            if (newV != null)
            {
                var newIconColor = (Color)newV;
                me?.IconColorChanged(oldIconColor, newIconColor);
            }
        });

        private void IconColorChanged(Color oldIconColor, Color newIconColor)
        {

        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public Color IconColor
        {
            get => (Color)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }
        #endregion

        public FloatingButton()
        {
            InitializeComponent();
        }
    }
}