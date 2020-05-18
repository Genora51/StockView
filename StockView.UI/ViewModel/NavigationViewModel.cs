using Prism.Events;
using StockView.Model;
using StockView.UI.Data;
using StockView.UI.Event;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IStockLookupDataService _stockLookupService;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IStockLookupDataService stockLookupService,
            IEventAggregator eventAggregator)
        {
            _stockLookupService = stockLookupService;
            _eventAggregator = eventAggregator;
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

        private LookupItem _selectedStock;

        public LookupItem SelectedStock
        {
            get { return _selectedStock; }
            set
            {
                _selectedStock = value;
                OnPropertyChanged();
                if(_selectedStock != null)
                {
                    _eventAggregator.GetEvent<OpenStockDetailViewEvent>()
                        .Publish(_selectedStock.Id);
                }
            }
        }

    }
}
