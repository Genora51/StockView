using StockView.Model;
using System;

namespace StockView.UI.Wrapper
{
    public class StockSnapshotWrapper : ModelWrapper<StockSnapshot>
    {
        public StockSnapshotWrapper(StockSnapshot model) : base(model)
        {
        }

        public DateTime Date
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }
        
        public float Value
        {
            get { return GetValue<float>(); }
            set { SetValue(value); }
        }
    }
}
