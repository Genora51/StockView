using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StockView.UI.ViewModel
{
    public class PageDataDetailViewModel : DetailViewModelBase
    {
        private IPageDataRepository _pageDataRepository;
        private PageWrapper _page;
        private StockSnapshotWrapper _selectedSnapshot;

        public PageDataDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPageDataRepository pageDataRepository)
            : base(eventAggregator, messageDialogService)
        {
            _pageDataRepository = pageDataRepository;

            Stocks = new ObservableCollection<StockWrapper>();
            StockSnapshots = new DataTable();
            // TODO: Reload snapshots on detail save/delete
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

        public StockSnapshotWrapper SelectedSnapshot
        {
            get { return _selectedSnapshot; }
            set
            {
                _selectedSnapshot = value;
                OnPropertyChanged();
                // TODO: ((DelegateCommand)RemoveSnapshotCommand).RaiseCanExecuteChanged();
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

        private void InitialisePage(Page page)
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

        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            // TODO: Snapshot errors
            return Page != null && !Page.HasErrors && HasChanges;
        }

        protected override void OnSaveExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}
