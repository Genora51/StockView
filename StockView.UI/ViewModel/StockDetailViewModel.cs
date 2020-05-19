using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data;
using StockView.UI.Event;
using StockView.UI.Wrapper;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : ViewModelBase, IStockDetailViewModel
    {
        private IStockDataService _dataService;
        private IEventAggregator _eventAggregator;
        private StockWrapper _stock;

        public StockDetailViewModel(IStockDataService dataService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenStockDetailViewEvent>()
                .Subscribe(OnOpenStockDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(int stockId)
        {
            var stock = await _dataService.GetByIdAsync(stockId);

            Stock = new StockWrapper(stock);
        }

        public StockWrapper Stock
        {
            get { return _stock; }
            private set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute()
        {
            // TODO: Check if stock is valid
            return true;
        }

        private async void OnSaveExecute()
        {
            await _dataService.SaveAsync(Stock.Model);
            _eventAggregator.GetEvent<AfterStockSavedEvent>().Publish(
                new AfterStockSavedEventArgs
                {
                    Id = Stock.Id,
                    DisplayMember = Stock.Symbol
                });
        }

        private async void OnOpenStockDetailView(int stockId)
        {
            await LoadAsync(stockId);
        }
    }
}
