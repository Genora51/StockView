using StockView.Model;
using StockView.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IStockLookupDataService _stockLookupService;

        public NavigationViewModel(IStockLookupDataService stockLookupService)
        {
            _stockLookupService = stockLookupService;
            Stocks = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var lookup = await _stockLookupService.GetStockLookupAsync();
            Stocks.Clear();
            foreach (var item in lookup)
            {
                Stocks.Add(item);
            }
        }

        public ObservableCollection<LookupItem> Stocks { get; }
    }
}
