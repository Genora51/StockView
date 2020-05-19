using Prism.Events;
using StockView.UI.Data.Lookups;
using StockView.UI.Event;
using System.Collections.ObjectModel;
using System.Linq;
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
            Stocks = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterStockSavedEvent>().Subscribe(AfterStockSaved);
        }

        private void AfterStockSaved(AfterStockSavedEventArgs obj)
        {
            var lookupItem = Stocks.Single(l => l.Id == obj.Id);
            lookupItem.DisplayMember = obj.DisplayMember;
        }

        public async Task LoadAsync()
        {
            var lookup = await _stockLookupService.GetStockLookupAsync();
            Stocks.Clear();
            foreach (var item in lookup)
            {
                Stocks.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                    _eventAggregator));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Stocks { get; }
    }
}
