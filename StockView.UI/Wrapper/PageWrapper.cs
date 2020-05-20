using StockView.Model;

namespace StockView.UI.Wrapper
{
    public class PageWrapper : ModelWrapper<Page>
    {
        public PageWrapper(Page model) : base(model)
        {
        }

        public int Id { get { return Model.Id; } }

        public string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
