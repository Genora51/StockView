using Prism.Commands;
using Prism.Events;
using StockView.Fetch;
using StockView.Model;
using StockView.UI.Data.Lookups;
using StockView.UI.Data.Repositories;
using StockView.UI.Event;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class StockDetailViewModel : DetailViewModelBase, IStockDetailViewModel
    {
        private readonly IStockRepository _stockRepository;
        private readonly IIndustryLookupDataService _industryLookupDataService;
        private readonly IStockDataFetchService _stockDataFetchService;
        private StockWrapper _stock;
        private StockSnapshotWrapper _selectedSnapshot;
        private int _changeCount;
        private bool _isFetching;

        public StockDetailViewModel(IStockRepository stockRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndustryLookupDataService industryLookupDataService,
            IStockDataFetchService stockDataFetchService)
            : base(eventAggregator, messageDialogService)
        {
            _stockRepository = stockRepository;
            _industryLookupDataService = industryLookupDataService;
            _stockDataFetchService = stockDataFetchService;

            eventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Subscribe(AfterCollectionSaved);
            eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(AfterDetailSaved);

            AddSnapshotCommand = new DelegateCommand(OnAddSnapshotExecute);
            RemoveSnapshotCommand = new DelegateCommand(OnRemoveSnapshotExecute, OnRemoveSnapshotCanExecute);
            FetchSnapshotCommand = new DelegateCommand(OnFetchSnapshotExecute, OnFetchSnapshotCanExecute);

            Industries = new ObservableCollection<LookupItem>();
            Snapshots = new ObservableCollection<StockSnapshotWrapper>();
            ChangeCount = 0;
        }

        public override async Task LoadAsync(int stockId)
        {
            var stock = stockId > 0
                ? await _stockRepository.GetByIdAsync(stockId)
                : CreateNewStock();

            Id = stockId;

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
                var wrapper = new StockSnapshotWrapper(stockSnapshot, ValidateSnapshotProperty);
                Snapshots.Add(wrapper);
                wrapper.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
            }
        }

        private IEnumerable<string> ValidateSnapshotProperty(string propertyName, object currentValue)
        {
            switch (propertyName)
            {
                case nameof(StockSnapshotWrapper.Date):
                    if (Snapshots.Count(s => s.Date == (DateTime)currentValue) > 1)
                    {
                        yield return "Date must be unique";
                    }
                    break;
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
            ChangeCount++;
        }

        private void InitialiseStock(Stock stock)
        {
            Stock = new StockWrapper(stock);
            Stock.PropertyChanged += StockWrapper_PropertyChanged;
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Stock.Id == 0)
            {
                // Trick to trigger validation
                Stock.Symbol = "";
            }
            SetTitle();
        }

        private void StockWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _stockRepository.HasChanges();
            }
            if (e.PropertyName == nameof(Stock.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
            if (e.PropertyName == nameof(Stock.Symbol))
            {
                SetTitle();
            }
        }

        private void SetTitle()
        {
            Title = Stock.Symbol;
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
            set
            {
                _selectedSnapshot = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddSnapshotCommand { get; }
        public ICommand RemoveSnapshotCommand { get; }
        public ICommand FetchSnapshotCommand { get; }

        public ObservableCollection<LookupItem> Industries { get; }

        public ObservableCollection<StockSnapshotWrapper> Snapshots { get; }

        public int ChangeCount
        {
            get { return _changeCount; }
            private set { _changeCount = value; OnPropertyChanged(); }
        }

        public bool IsFetching
        {
            get { return _isFetching; }
            private set
            {
                _isFetching = value;
                ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
            }
        }

        private ICollectionView _snapshotsView;


        public ICollectionView SnapshotsView
        {
            get
            {
                if (_snapshotsView == null)
                {
                    _snapshotsView = CollectionViewSource.GetDefaultView(Snapshots);
                    _snapshotsView?.SortDescriptions?.Add(
                        new SortDescription("Date", ListSortDirection.Ascending)
                    );
                }
                return _snapshotsView;
            }
        }


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
            await SaveWithOptimisticConcurrencyAsync(_stockRepository.SaveAsync,
                () =>
                {
                    HasChanges = _stockRepository.HasChanges();
                    Id = Stock.Id;
                    RaiseDetailSavedEvent(Stock.Id, Stock.Symbol);
                });
        }

        protected override async void OnDeleteExecute()
        {
            if (await _stockRepository.HasPagesAsync(Stock.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync($"{Stock.Symbol} can't be deleted as it is part of at least one page.");
                return;
            }
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Do you really want to delete the stock {Stock.Symbol}?",
                "Question");
            if (result == MessageDialogResult.OK)
            {

                _stockRepository.Remove(Stock.Model);
                await _stockRepository.SaveAsync();
                RaiseDetailDeletedEvent(Id);
            }
        }

        private void OnAddSnapshotExecute()
        {
            var newSnapshot = new StockSnapshotWrapper(new StockSnapshot
            {
                Date = new DateTime(1970, 1, 1)
            }, ValidateSnapshotProperty);
            newSnapshot.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
            Snapshots.Add(newSnapshot);
            Stock.Model.Snapshots.Add(newSnapshot.Model);
            HasChanges = _stockRepository.HasChanges();
            // Trigger validation
            newSnapshot.Date = DateTime.Now.Date;
            SnapshotsView.Refresh();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnRemoveSnapshotExecute()
        {
            SelectedSnapshot.PropertyChanged -= StockSnapshotWrapper_PropertyChanged;
            _stockRepository.RemoveSnapshot(SelectedSnapshot.Model);
            Snapshots.Remove(SelectedSnapshot);
            SelectedSnapshot = null;
            HasChanges = _stockRepository.HasChanges();
            SnapshotsView.Refresh();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnRemoveSnapshotCanExecute()
        {
            return SelectedSnapshot != null;
        }

        private async void OnFetchSnapshotExecute()
        {
            IsFetching = true;
            // Store selected snapshot now to prevent incorrect cell being filled
            var selectedSnapshot = SelectedSnapshot;
            var fetchedSnapshot = await _stockDataFetchService.FetchSnapshotAsync(Stock.Model, SelectedSnapshot.Date);
            if (fetchedSnapshot == null)
            {
                await MessageDialogService.ShowInfoDialogAsync("No data available for this date.");
            }
            else
            {
                selectedSnapshot.Value = fetchedSnapshot.Value;
                selectedSnapshot.ExDividends = fetchedSnapshot.ExDividends;
            }
            IsFetching = false;
        }

        private bool OnFetchSnapshotCanExecute()
        {
            return SelectedSnapshot != null && !IsFetching;
        }

        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(IndustryDetailViewModel))
            {
                await LoadIndustriesLookupAsync();
            }
        }

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(PageDataDetailViewModel))
            {
                if (await _stockRepository.BelongsToPageAsync(Stock.Id, args.Id))
                {
                    _stockRepository.DetachStock(Stock.Model);
                    Stock.PropertyChanged -= StockWrapper_PropertyChanged;
                    await LoadAsync(Stock.Id);
                    SelectedSnapshot = null;
                    HasChanges = _stockRepository.HasChanges();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }
    }
}
