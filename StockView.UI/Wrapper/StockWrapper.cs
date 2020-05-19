using StockView.Model;
using System;

namespace StockView.UI.Wrapper
{
    public class StockWrapper : NotifyDataErrorInfoBase
    {
        public StockWrapper(Stock model)
        {
            Model = model;
        }

        public Stock Model { get; }

        public int Id { get { return Model.Id; } }

        public string Symbol
        {
            get { return Model.Symbol; }
            set
            {
                Model.Symbol = value;
                OnPropertyChanged();
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
            get { return Model.CompanyName; }
            set
            {
                Model.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public string Industry
        {
            get { return Model.Industry; }
            set
            {
                Model.Industry = value;
                OnPropertyChanged();
            }
        }
    }
}
