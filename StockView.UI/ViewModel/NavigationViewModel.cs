using Prism.Events;
using StockView.UI.Data.Lookups;
using StockView.UI.Event;
using System;
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
            _eventAggregator.GetEvent<AfterStockDeletedEvent>().Subscribe(AfterStockDeleted);
        }

        public async Task LoadAsync()
        {
            var lookup = await _stockLookupService.GetStockLookupAsync();
            Stocks.Clear();
            foreach (var item in lookup)
            {
                Stocks.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                    nameof(StockDetailViewModel),
                    _eventAggregator));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Stocks { get; }

        private void AfterStockDeleted(int stockId)
        {
            var stock = Stocks.SingleOrDefault(s => s.Id == stockId);
            if (stock != null)
            {
                Stocks.Remove(stock);
            }
        }

        private void AfterStockSaved(AfterStockSavedEventArgs obj)
        {
            var lookupItem = Stocks.SingleOrDefault(l => l.Id == obj.Id);
            if (lookupItem == null)
            {
                Stocks.Add(new NavigationItemViewModel(obj.Id, obj.DisplayMember,
                    nameof(StockDetailViewModel),
                    _eventAggregator));
            }
            else
            {
                lookupItem.DisplayMember = obj.DisplayMember;
            }
        }
    }
}
