using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Lookups;
using StockView.UI.Data.Repositories;
using StockView.UI.Event;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : ViewModelBase, IStockDetailViewModel
    {
        private IStockRepository _stockRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IIndustryLookupDataService _industryLookupDataService;
        private StockWrapper _stock;
        private bool _hasChanges;

        public StockDetailViewModel(IStockRepository stockRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndustryLookupDataService industryLookupDataService)
        {
            _stockRepository = stockRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _industryLookupDataService = industryLookupDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);

            Industries = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync(int? stockId)
        {
            var stock = stockId.HasValue
                ? await _stockRepository.GetByIdAsync(stockId.Value)
                : CreateNewStock();
            InitialiseStock(stock);

            await LoadIndustriesLookupAsync();
        }

        private void InitialiseStock(Stock stock)
        {
            Stock = new StockWrapper(stock);
            Stock.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _stockRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Stock.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Stock.Id == 0)
            {
                // Trick to trigger validation
                Stock.Symbol = "";
            }
        }

        private async Task LoadIndustriesLookupAsync()
        {
            Industries.Clear();
            var lookup = await _industryLookupDataService.GetIndustryLookupAsync();
            foreach (var lookupItem in lookup)
            {
                Industries.Add(lookupItem);
            }
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
        public ICommand DeleteCommand { get; }
        public ObservableCollection<LookupItem> Industries { get; }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set {
                if (_hasChanges!=value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private bool OnSaveCanExecute()
        {
            return Stock != null && !Stock.HasErrors && HasChanges;
        }

        private Stock CreateNewStock()
        {
            var stock = new Stock();
            _stockRepository.Add(stock);
            return stock;
        }

        private async void OnSaveExecute()
        {
            await _stockRepository.SaveAsync();
            HasChanges = _stockRepository.HasChanges();
            _eventAggregator.GetEvent<AfterStockSavedEvent>().Publish(
                new AfterStockSavedEventArgs
                {
                    Id = Stock.Id,
                    DisplayMember = Stock.Symbol
                });
        }

        private async void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete the stock {Stock.Symbol}?",
                "Question");
            if (result == MessageDialogResult.OK)
            {
                _stockRepository.Remove(Stock.Model);
                await _stockRepository.SaveAsync();
                _eventAggregator.GetEvent<AfterStockDeletedEvent>().Publish(Stock.Id);
            }
        }
    }
}
