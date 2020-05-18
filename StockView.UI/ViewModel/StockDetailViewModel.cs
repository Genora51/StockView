using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data;
using StockView.UI.Event;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : ViewModelBase, IStockDetailViewModel
    {
        private IStockDataService _dataService;
        private IEventAggregator _eventAggregator;

        public StockDetailViewModel(IStockDataService dataService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenStockDetailViewEvent>()
                .Subscribe(OnOpenStockDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private bool OnSaveCanExecute()
        {
            // TODO: Check if stock is valid
            return true;
        }

        private void OnSaveExecute()
        {
            throw new NotImplementedException();
        }

        private async void OnOpenStockDetailView(int stockId)
        {
            await LoadAsync(stockId);
        }

        public async Task LoadAsync(int stockId)
        {
            Stock = await _dataService.GetByIdAsync(stockId);
        }

        private Stock _stock;

        public Stock Stock
        {
            get { return _stock; }
            private set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
    }
}
