using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Lookups;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : DetailViewModelBase, IStockDetailViewModel
    {
        private IStockRepository _stockRepository;
        private IMessageDialogService _messageDialogService;
        private IIndustryLookupDataService _industryLookupDataService;
        private StockWrapper _stock;
        private StockSnapshotWrapper _selectedSnapshot;

        public StockDetailViewModel(IStockRepository stockRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndustryLookupDataService industryLookupDataService)
            : base(eventAggregator)
        {
            _stockRepository = stockRepository;
            _messageDialogService = messageDialogService;
            _industryLookupDataService = industryLookupDataService;

            AddSnapshotCommand = new DelegateCommand(OnAddSnapshotExecute);
            RemoveSnapshotCommand = new DelegateCommand(OnRemoveSnapshotExecute, OnRemoveSnapshotCanExecute);

            Industries = new ObservableCollection<LookupItem>();
            Snapshots = new ObservableCollection<StockSnapshotWrapper>();
        }

        public override async Task LoadAsync(int? stockId)
        {
            var stock = stockId.HasValue
                ? await _stockRepository.GetByIdAsync(stockId.Value)
                : CreateNewStock();
            InitialiseStock(stock);

            InitialiseStockSnapshots(stock.Snapshots);

            await LoadIndustriesLookupAsync();
        }

        private void InitialiseStockSnapshots(ICollection<StockSnapshot> snapshots)
        {
            foreach (var wrapper in Snapshots)
            {
                wrapper.PropertyChanged -= StockSnapshotWrapper_PropertyChanged;
            }
            Snapshots.Clear();
            foreach (var stockSnapshot in snapshots)
            {
                var wrapper = new StockSnapshotWrapper(stockSnapshot);
                Snapshots.Add(wrapper);
                wrapper.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
            }
        }

        private void StockSnapshotWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _stockRepository.HasChanges();
            }
            if (e.PropertyName == nameof(StockSnapshotWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
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
            Industries.Add(new NullLookupItem { DisplayMember = " - " });
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

        public StockSnapshotWrapper SelectedSnapshot
        {
            get { return _selectedSnapshot; }
            set {
                _selectedSnapshot = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddSnapshotCommand { get; }
        public ICommand RemoveSnapshotCommand { get; }

        public ObservableCollection<LookupItem> Industries { get; }

        public ObservableCollection<StockSnapshotWrapper> Snapshots { get; }

        protected override bool OnSaveCanExecute()
        {
            return Stock != null
                && !Stock.HasErrors
                && Snapshots.All(s => !s.HasErrors)
                && HasChanges;
        }

        private Stock CreateNewStock()
        {
            var stock = new Stock();
            _stockRepository.Add(stock);
            return stock;
        }

        protected override async void OnSaveExecute()
        {
            await _stockRepository.SaveAsync();
            HasChanges = _stockRepository.HasChanges();
            RaiseDetailSavedEvent(Stock.Id, Stock.Symbol);
        }

        protected override async void OnDeleteExecute()
        {
            if (await _stockRepository.HasPagesAsync(Stock.Id))
            {
                _messageDialogService.ShowInfoDialog($"{Stock.Symbol} can't be deleted as it is part of at least one page.");
                return;
            }
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to delete the stock {Stock.Symbol}?",
                "Question");
            if (result == MessageDialogResult.OK)
            {
                _stockRepository.Remove(Stock.Model);
                await _stockRepository.SaveAsync();
                RaiseDetailDeletedEvent(Stock.Id);
            }
        }

        private void OnAddSnapshotExecute()
        {
            var newSnapshot = new StockSnapshotWrapper(new StockSnapshot {
                Date = DateTime.Now
            });
            newSnapshot.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
            Snapshots.Add(newSnapshot);
            Stock.Model.Snapshots.Add(newSnapshot.Model);
            HasChanges = _stockRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnRemoveSnapshotExecute()
        {
            SelectedSnapshot.PropertyChanged -= StockSnapshotWrapper_PropertyChanged;
            _stockRepository.RemoveSnapshot(SelectedSnapshot.Model);
            Snapshots.Remove(SelectedSnapshot);
            SelectedSnapshot = null;
            HasChanges = _stockRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnRemoveSnapshotCanExecute()
        {
            return SelectedSnapshot != null;
        }
    }
}
