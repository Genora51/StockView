﻿using Prism.Commands;
using Prism.Events;
using StockView.UI.Data.Repositories;
using StockView.UI.Event;
using StockView.UI.Wrapper;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : ViewModelBase, IStockDetailViewModel
    {
        private IStockRepository _stockRepository;
        private IEventAggregator _eventAggregator;
        private StockWrapper _stock;

        public StockDetailViewModel(IStockRepository stockRepository,
            IEventAggregator eventAggregator)
        {
            _stockRepository = stockRepository;
            _eventAggregator = eventAggregator;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(int stockId)
        {
            var stock = await _stockRepository.GetByIdAsync(stockId);

            Stock = new StockWrapper(stock);
            Stock.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Stock.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
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
            // TODO: Check in addition if stock has changes
            return Stock != null && !Stock.HasErrors;
        }

        private async void OnSaveExecute()
        {
            await _stockRepository.SaveAsync();
            _eventAggregator.GetEvent<AfterStockSavedEvent>().Publish(
                new AfterStockSavedEventArgs
                {
                    Id = Stock.Id,
                    DisplayMember = Stock.Symbol
                });
        }
    }
}
