using StockView.Model;

namespace StockView.UI.Wrapper
{
    public class IndustryWrapper : ModelWrapper<Industry>
    {
        public IndustryWrapper(Industry model) : base(model)
        {
        }

        public int Id { get { return Model.Id; } }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
