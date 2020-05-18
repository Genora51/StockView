using StockView.Model;
using StockView.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IStockDataService _stockDataService;
        private Stock _selectedStock;

        public MainViewModel(IStockDataService stockDataService)
        {
            Stocks = new ObservableCollection<Stock>();
            _stockDataService = stockDataService;
        }

        public async Task LoadAsync()
        {
            var stocks = await _stockDataService.GetAllAsync();
            Stocks.Clear();
            foreach (var stock in stocks)
            {
                Stocks.Add(stock);
            }
        }
        public ObservableCollection<Stock> Stocks { get; set; }

        public Stock SelectedStock
        {
            get { return _selectedStock; }
            set {
                _selectedStock = value;
                OnPropertyChanged();
            }
        }
    }
}
