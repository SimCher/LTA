using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LTA.Mobile.Client.Styles.MarkupExtensions
{
    public class ImageResourceMarkupExtension : IMarkupExtension
    {
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;

            var imageSrc =
                ImageSource.FromResource(Source, typeof(ImageResourceMarkupExtension).Assembly);

            return imageSrc;
        }
    }
}