using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.UI.DataTemplates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorTemplate
    {
        public static readonly BindableProperty ErrorMessageProperty =
            BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(ErrorTemplate), string.Empty);

        public ErrorTemplate()
        {
            InitializeComponent();
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }
    }
}