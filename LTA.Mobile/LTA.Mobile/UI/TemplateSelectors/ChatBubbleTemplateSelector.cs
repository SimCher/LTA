using LTA.Mobile.Domain.Models;
using LTA.Mobile.UI.DataTemplates;
using Xamarin.Forms;

namespace LTA.Mobile.UI.TemplateSelectors;

public class ChatBubbleTemplateSelector : DataTemplateSelector
{
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is Message { IsSent: true })
        {
            var messageTemplate = new MessageIsSentTemplate
            {
                ParentContext = container.BindingContext
            };
            return new DataTemplate(() => messageTemplate);
        }
        else
        {
            var messageTemplate = new MessagePeerSentTemplate
            {
                ParentContext = container.BindingContext
            };

            return new DataTemplate(() => messageTemplate);
        }
    }
}