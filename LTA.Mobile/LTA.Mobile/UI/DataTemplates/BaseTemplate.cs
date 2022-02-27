using Xamarin.Forms;

namespace LTA.Mobile.UI.DataTemplates;

public abstract class BaseTemplate : ContentView
{
    #region ParentContext

    public static readonly BindableProperty ParentContextProperty;

    static BaseTemplate()
    {
        ParentContextProperty = BindableProperty.Create(nameof(ParentContext),
            typeof(object), typeof(BaseTemplate), propertyChanged: (obj, old, newV) =>
            {
                var me = obj as BaseTemplate;
                me?.ParentContextChanged(old, newV);
            });
    }

    public object ParentContext
    {
        get => GetValue(ParentContextProperty);
        set => SetValue(ParentContextProperty, value);
    }

    private void ParentContextChanged(object oldParentContext, object newParentContext)
    { }

    #endregion
}