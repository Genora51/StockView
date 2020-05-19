using StockView.Model;
using System;

namespace StockView.UI.Wrapper
{
    public class StockWrapper : ModelWrapper<Stock> {
        
        public StockWrapper(Stock model) : base(model)
        {
        }
        public int Id { get { return Model.Id; } }

        public string Symbol
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
                ValidateProperty(nameof(Symbol));
            }
        }

        private void ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);
            switch(propertyName)
            {
                case nameof(Symbol):
                    if (string.Equals(Symbol, "INVL", StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(propertyName, "Invalid Symbol");
                    }
                    break;
            }
        }

        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Industry
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
