using StockView.Model;

namespace StockView.UI.Wrapper
{
    public class SummaryWrapper : ModelWrapper<Summary>
    {
        public SummaryWrapper(Summary model) : base(model)
        {
        }

        public int Id { get { return Model.Id; } }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Code
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool Enabled
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public int SortIndex
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}
