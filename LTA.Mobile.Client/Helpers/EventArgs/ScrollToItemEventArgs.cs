namespace LTA.Mobile.Client.Helpers.EventArgs
{
    public class ScrollToItemEventArgs : System.EventArgs
    {
        public object Item { get; set; }
        public int? Index { get; set; }
    }
}