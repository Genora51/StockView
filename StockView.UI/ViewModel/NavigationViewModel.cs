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
        private IPageLookupDataService _pageLookupService;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IStockLookupDataService stockLookupService,
            IPageLookupDataService pageLookupService,
            IEventAggregator eventAggregator)
        {
            _stockLookupService = stockLookupService;
            _pageLookupService = pageLookupService;
            _eventAggregator = eventAggregator;
            Stocks = new ObservableCollection<NavigationItemViewModel>();
            Pages = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
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
            lookup = await _pageLookupService.GetPageLookupAsync();
            Pages.Clear();
            foreach (var item in lookup)
            {
                Pages.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                    nameof(PageDataDetailViewModel),
                    _eventAggregator));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Stocks { get; }

        public ObservableCollection<NavigationItemViewModel> Pages { get; }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(StockDetailViewModel):
                    AfterDetailDeleted(Stocks, args);
                    break;
                case nameof(PageDetailViewModel):
                case nameof(PageDataDetailViewModel):
                    AfterDetailDeleted(Pages, args);
                    break;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var item = items.SingleOrDefault(s => s.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(StockDetailViewModel):
                    AfterDetailSaved(Stocks, args);
                    break;
                case nameof(PageDetailViewModel):
                case nameof(PageDataDetailViewModel):
                    AfterDetailSaved(Pages, args);
                    break;
            }
        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSavedEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(l => l.Id == args.Id);
            if (lookupItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember,
                    args.ViewModelName,
                    _eventAggregator));
            }
            else
            {
                lookupItem.DisplayMember = args.DisplayMember;
            }
        }
    }
}
