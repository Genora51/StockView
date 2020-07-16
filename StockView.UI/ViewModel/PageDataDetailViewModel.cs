using Prism.Commands;
using Prism.Events;
using StockView.Fetch;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.Event;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class PageDataDetailViewModel : DetailViewModelBase
    {
        private IPageDataRepository _pageDataRepository;
        private PageWrapper _page;
        private DataGridCellInfo _selectedCell;
        private IStockDataFetchService _stockDataFetchService;

        public PageDataDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPageDataRepository pageDataRepository,
            IStockDataFetchService stockDataFetchService)
            : base(eventAggregator, messageDialogService)
        {
            _pageDataRepository = pageDataRepository;
            _stockDataFetchService = stockDataFetchService;
            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);

            Stocks = new ObservableCollection<StockWrapper>();
            StockSnapshots = new DataTable();
            ChangeCount = 0;
            // Assign delegate commands
            OpenPageDetailViewCommand = new DelegateCommand(OnOpenPageDetailViewExecute);
            AddSnapshotCommand = new DelegateCommand(OnAddSnapshotExecute, OnAddSnapshotCanExecute);
            RemoveSnapshotCommand = new DelegateCommand(OnRemoveSnapshotExecute, OnRemoveSnapshotCanExecute);
            AddRowCommand = new DelegateCommand(OnAddRowExecute);
            RemoveRowCommand = new DelegateCommand(OnRemoveRowExecute, OnRemoveRowCanExecute);

            // Fetch commands
            FetchSnapshotCommand = new DelegateCommand(OnFetchSnapshotExecute, OnFetchSnapshotCanExecute);
            FetchRowCommand = new DelegateCommand(OnFetchRowExecute, OnFetchRowCanExecute);
        }

        public PageWrapper Page
        {
            get { return _page; }
            private set
            {
                _page = value;
                OnPropertyChanged();
            }
        }

        public DataGridCellInfo SelectedCell
        {
            get { return _selectedCell; }
            set
            {
                if (value.Column == null) return;
                _selectedCell = value;
                OnPropertyChanged();
                ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)RemoveRowCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)FetchRowCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
            }
        }

        private StockSnapshotWrapper SelectedSnapshot
        {
            get
            {
                if (SelectedCell == null || SelectedCell.Column == null) return null;
                var stockName = SelectedCell.Column.Header.ToString();
                var item = ((DataRowView)SelectedCell.Item)[stockName];
                if (item is StockSnapshotWrapper snapshot) return snapshot;
                else return null;
            }
        }

        public ObservableCollection<StockWrapper> Stocks { get; }
        public DataTable StockSnapshots { get; }
        private int _changeCount;

        public int ChangeCount
        {
            get { return _changeCount; }
            private set { _changeCount = value; OnPropertyChanged(); }
        }


        public async override Task LoadAsync(int pageId)
        {
            var page = await _pageDataRepository.GetByIdAsync(pageId);

            Id = pageId;
            InitialisePage(page);
            InitialisePageStocks(page.Stocks);
            InitialisePageSnapshots(page.Stocks);
        }

        private void InitialisePage(Model.Page page)
        {
            Page = new PageWrapper(page);
            Page.PropertyChanged += PageWrapper_PropertyChanged;
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            Title = Page.Title;
        }

        private void PageWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _pageDataRepository.HasChanges();
            }

            if (e.PropertyName == nameof(Page.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void InitialisePageStocks(ICollection<Stock> stocks)
        {
            Stocks.Clear();
            foreach (var stock in stocks)
            {
                var wrapper = new StockWrapper(stock);
                Stocks.Add(wrapper);
            }
        }

        private void InitialisePageSnapshots(ICollection<Stock> stocks)
        {
            StockSnapshots.ColumnChanged -= StockSnapshots_ColumnChanged;
            foreach (DataRow row in StockSnapshots.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    if (item is StockSnapshotWrapper wrapper)
                        wrapper.PropertyChanged -= StockSnapshotWrapper_PropertyChanged;
                }
            }
            StockSnapshots.Rows.Clear();
            StockSnapshots.DefaultView.Sort = "";
            StockSnapshots.Columns.Clear();
            // Set up snapshots
            StockSnapshots.Columns.Add("Date", typeof(DateTime));
            StockSnapshots.DefaultView.Sort = "Date ASC";
            foreach (var stock in stocks)
            {
                StockSnapshots.Columns.Add(stock.Symbol, typeof(StockSnapshotWrapper));
            }
            var snaps = stocks.SelectMany(
                s => s.Snapshots.Select(
                    snap => new { s.Symbol, Snapshot = snap }
                )
            );
            var rows = from sn in snaps
                       group sn by sn.Snapshot.Date into g
                       select new { Date = g.Key, Snapshots = g.AsEnumerable() };
            foreach (var row in rows)
            {
                var dataRow = StockSnapshots.NewRow();
                dataRow["Date"] = row.Date;
                foreach (var snapObj in row.Snapshots)
                {
                    var wrapper = new StockSnapshotWrapper(snapObj.Snapshot);
                    wrapper.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
                    dataRow[snapObj.Symbol] = wrapper;
                }
                StockSnapshots.Rows.Add(dataRow);
            }
            StockSnapshots.ColumnChanged += StockSnapshots_ColumnChanged;
        }

        private void StockSnapshotWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _pageDataRepository.HasChanges();
            }
            if (e.PropertyName == nameof(StockSnapshotWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
            ChangeCount++;
        }

        private void StockSnapshots_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.DataType == typeof(DateTime))
            {
                if (StockSnapshots.Rows.OfType<DataRow>().Count(
                        r => (DateTime)r["Date"] == (DateTime)e.ProposedValue
                    ) > 1
                ) {
                    e.Row.SetColumnError(e.Column, "Date must be unique");
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                    return;
                } else {
                    e.Row.ClearErrors();
                }
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                foreach (var item in e.Row.ItemArray)
                {
                    if (item is StockSnapshotWrapper wrapper)
                        wrapper.Date = (DateTime) e.ProposedValue;
                }
            }
            ChangeCount++;
        }

        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return Page != null
                && !Page.HasErrors
                && !StockSnapshots.HasErrors
                && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_pageDataRepository.SaveAsync,
                () =>
                {
                    HasChanges = _pageDataRepository.HasChanges();
                    Id = Page.Id;
                    RaiseDetailSavedEvent(Page.Id, Page.Title);
                });
        }

        public ICommand OpenPageDetailViewCommand { get; }
        public ICommand AddSnapshotCommand { get; }
        public ICommand RemoveSnapshotCommand { get; }
        public ICommand FetchSnapshotCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand FetchRowCommand { get; }

        private void AddSnapshotInPlace(StockSnapshot snapshot)
        {
            var row = (DataRowView)SelectedCell.Item;
            var newSnapshot = new StockSnapshotWrapper(snapshot);
            newSnapshot.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
            var symbol = SelectedCell.Column.Header.ToString();
            row[symbol] = newSnapshot;
            Stocks.First(
                s => s.Symbol == symbol
            ).Model.Snapshots.Add(newSnapshot.Model);
            HasChanges = _pageDataRepository.HasChanges();
            //SelectedCell
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
        }
        private void OnOpenPageDetailViewExecute()
        {
            EventAggregator.GetEvent<OpenDetailViewEvent>().Publish(new OpenDetailViewEventArgs
            {
                Id = Page.Id,
                ViewModelName = nameof(PageDetailViewModel)
            });
        }
        private void OnAddSnapshotExecute()
        {
            var row = (DataRowView)SelectedCell.Item;
            AddSnapshotInPlace(new StockSnapshot
            {
                Date = (DateTime)row["Date"],
            });
        }
        private bool OnAddSnapshotCanExecute()
        {
            return SelectedSnapshot == null
                && SelectedCell.Item != null
                && SelectedCell.Column.Header.ToString() != "Date";
        }
        private void OnRemoveSnapshotExecute()
        {
            SelectedSnapshot.PropertyChanged -= StockSnapshotWrapper_PropertyChanged;
            _pageDataRepository.RemoveSnapshot(SelectedSnapshot.Model);
            var row = (DataRowView)SelectedCell.Item;
            var symbol = SelectedCell.Column.Header.ToString();
            row[symbol] = null;
            if (row.Row.ItemArray.OfType<StockSnapshotWrapper>().Count() == 0)
            {
                _selectedCell = default;
                row.Delete();
                ((DelegateCommand)RemoveRowCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)FetchRowCommand).RaiseCanExecuteChanged();
            }
            HasChanges = _pageDataRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
        }
        private bool OnRemoveSnapshotCanExecute()
        {
            return SelectedSnapshot != null;
        }
        private void OnAddRowExecute()
        {
            var row = StockSnapshots.NewRow();
            StockSnapshots.Rows.Add(row);
            // Trigger validation
            row["Date"] = DateTime.Now.Date;
            ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveRowCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchRowCommand).RaiseCanExecuteChanged();
        }
        private void OnRemoveRowExecute()
        {
            var row = ((DataRowView)SelectedCell.Item).Row;
            foreach (var snapshot in row.ItemArray.OfType<StockSnapshotWrapper>())
            {
                snapshot.PropertyChanged -= StockSnapshotWrapper_PropertyChanged;
                _pageDataRepository.RemoveSnapshot(snapshot.Model);
            }
            _selectedCell = default;
            row.Delete();
            HasChanges = _pageDataRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveRowCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchRowCommand).RaiseCanExecuteChanged();
        }
        private bool OnRemoveRowCanExecute()
        {
            return SelectedCell.Item is DataRowView;
        }
        private async void OnFetchSnapshotExecute()
        {
            var symbol = SelectedCell.Column.Header.ToString();
            var stock = Stocks.First(
                s => s.Symbol == symbol
            ).Model;
            var row = (DataRowView)SelectedCell.Item;
            var date = (DateTime)row["Date"];
            var fetchedSnapshot = await _stockDataFetchService.FetchSnapshotAsync(stock, date);
            if (fetchedSnapshot == null)
            {
                await MessageDialogService.ShowInfoDialogAsync("No data available for this date.");
            } else
            {
                if (SelectedSnapshot == null)
                {
                    AddSnapshotInPlace(fetchedSnapshot);
                }
                else
                {
                    SelectedSnapshot.Value = fetchedSnapshot.Value;
                    SelectedSnapshot.ExDividends = fetchedSnapshot.ExDividends;
                }
            }
        }
        private bool OnFetchSnapshotCanExecute()
        {
            return SelectedSnapshot != null || (SelectedCell.Item != null && SelectedCell.Column.Header.ToString() != "Date");
        }
        private void OnFetchRowExecute()
        {
            // TODO: Implement
        }
        private bool OnFetchRowCanExecute()
        {
            return SelectedCell.Item is DataRowView;
        }

        private async Task ReloadPage()
        {
            _pageDataRepository.DetachPage(Page.Model);
            Page.PropertyChanged -= PageWrapper_PropertyChanged;
            _selectedCell = default;
            await LoadAsync(Page.Id);
            HasChanges = _pageDataRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveRowCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchRowCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)FetchSnapshotCommand).RaiseCanExecuteChanged();
        }

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(StockDetailViewModel):
                    // FIXME: why was this called even after VM is closed?
                    if (Stocks.Any(s => s.Id == args.Id)) await ReloadPage();
                    break;
                case nameof(PageDetailViewModel):
                    if (args.Id == Page.Id) await ReloadPage();
                    break;
            }
        }

        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(StockDetailViewModel):
                    if (Stocks.Any(s => s.Id == args.Id)) await ReloadPage();
                    break;
                case nameof(PageDetailViewModel):
                    if (args.Id == Page.Id)
                    {
                        EventAggregator.GetEvent<AfterDetailClosedEvent>()
                            .Publish(new AfterDetailClosedEventArgs
                            {
                                Id = Page.Id,
                                ViewModelName = this.GetType().Name
                            });
                    }
                    break;
            }
        }
    }
}
