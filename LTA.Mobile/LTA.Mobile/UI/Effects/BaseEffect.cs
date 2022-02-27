using Xamarin.Forms;

namespace LTA.Mobile.UI.Effects;

public abstract class BaseEffect : RoutingEffect
{
    protected BaseEffect(string effectId) : base($"LTA.{effectId}")
    {
    }
}