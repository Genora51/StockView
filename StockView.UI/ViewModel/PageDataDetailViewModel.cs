using Prism.Commands;
using Prism.Events;
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

        public PageDataDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPageDataRepository pageDataRepository)
            : base(eventAggregator, messageDialogService)
        {
            // TODO: Concurrency, Validation
            _pageDataRepository = pageDataRepository;

            Stocks = new ObservableCollection<StockWrapper>();
            StockSnapshots = new DataTable();
            // TODO: Reload snapshots on detail save/delete
            // Assign delegate commands
            OpenPageDetailViewCommand = new DelegateCommand(OnOpenPageDetailViewExecute);
            AddSnapshotCommand = new DelegateCommand(OnAddSnapshotExecute, OnAddSnapshotCanExecute);
            RemoveSnapshotCommand = new DelegateCommand(OnRemoveSnapshotExecute, OnRemoveSnapshotCanExecute);
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
            Page.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _pageDataRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Page.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            Title = Page.Title;
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
            StockSnapshots.Columns.Clear();
            // Set up snapshots
            StockSnapshots.Columns.Add("Date", typeof(DateTime));
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
            var newSnapshot = new StockSnapshotWrapper(new StockSnapshot
            {
                Date = (DateTime)row["Date"],
            });
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
            }
            HasChanges = _pageDataRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)AddSnapshotCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();

        }
        private bool OnRemoveSnapshotCanExecute()
        {
            return SelectedSnapshot != null;
        }
    }
}
