using Prism.Events;

namespace StockView.UI.Event
{
    public class AfterStockSavedEvent : PubSubEvent<AfterStockSavedEventArgs>
    {
    }

    public class AfterStockSavedEventArgs
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
