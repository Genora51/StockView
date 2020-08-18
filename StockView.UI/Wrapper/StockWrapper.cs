using StockView.Model;
using System;
using System.Collections.Generic;

namespace StockView.UI.Wrapper
{
    public class StockWrapper : ModelWrapper<Stock>
    {

        public StockWrapper(Stock model) : base(model)
        {
        }
        public int Id { get { return Model.Id; } }

        public string Symbol
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int? IndustryId
        {
            get { return GetValue<int?>(); }
            set { SetValue(value); }
        }

        public int Shares
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Symbol):
                    if (Symbol.Equals("Date", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Disallowed symbol";
                    }
                    break;
            }
        }
    }
}
