using StockView.Model;
using System;
using System.Collections.Generic;

namespace StockView.UI.Wrapper
{
    public class StockSnapshotWrapper : ModelWrapper<StockSnapshot>
    {
        public StockSnapshotWrapper(StockSnapshot model,
            Func<string, object, IEnumerable<string>> validatePropertyExternal = null)
            : base(model, validatePropertyExternal)
        {
        }

        public DateTime Date
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }
        
        public decimal Value
        {
            get { return GetValue<decimal>(); }
            set { SetValue(value); }
        }

        public bool ExDividends
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }
}
