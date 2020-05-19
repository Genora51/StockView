using StockView.Model;
using StockView.UI.ViewModel;

namespace StockView.UI.Wrapper
{
    public class StockWrapper : ViewModelBase
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
