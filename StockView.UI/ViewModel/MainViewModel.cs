using Prism.Events;
using StockView.UI.Event;
using System;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Func<IStockDetailViewModel> _stockDetailViewModelCreator;
        private IStockDetailViewModel _stockDetailViewModel;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IStockDetailViewModel> stockDetailViewModelCreator,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _stockDetailViewModelCreator = stockDetailViewModelCreator;

            _eventAggregator.GetEvent<OpenStockDetailViewEvent>()
                .Subscribe(OnOpenStockDetailView);

            NavigationViewModel = navigationViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public INavigationViewModel NavigationViewModel { get; }

        public IStockDetailViewModel StockDetailViewModel
        {
            get { return _stockDetailViewModel; }
            private set {
                _stockDetailViewModel = value;
                OnPropertyChanged();
            }
        }


        private async void OnOpenStockDetailView(int stockId)
        {
            StockDetailViewModel = _stockDetailViewModelCreator();
            await StockDetailViewModel.LoadAsync(stockId);
        }
    }
}
