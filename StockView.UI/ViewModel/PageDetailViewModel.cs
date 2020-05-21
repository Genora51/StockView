using Prism.Commands;
using Prism.Events;
using StockView.Model;
using StockView.UI.Data.Repositories;
using StockView.UI.View.Services;
using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockView.UI.ViewModel
{
    public class PageDetailViewModel : DetailViewModelBase, IPageDetailViewModel
    {
        private IPageRepository _pageRepository;
        private PageWrapper _page;

        private Stock _selectedAvailableStock;
        private Stock _selectedAddedStock;
        private IEnumerable<Stock> _allStocks;

        public PageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPageRepository pageRepository) : base(eventAggregator, messageDialogService)
        {
            _pageRepository = pageRepository;

            AddedStocks = new ObservableCollection<Stock>();
            AvailableStocks = new ObservableCollection<Stock>();
            AddStockCommand = new DelegateCommand(OnAddStockExecute, OnAddStockCanExecute);
            RemoveStockCommand = new DelegateCommand(OnRemoveStockExecute, OnRemoveStockCanExecute);
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

        public ICommand AddStockCommand { get; }
        public ICommand RemoveStockCommand { get; }

        public ObservableCollection<Stock> AddedStocks { get; }
        public ObservableCollection<Stock> AvailableStocks { get; }

        public Stock SelectedAvailableStock
        {
            get { return _selectedAvailableStock; }
            set
            {
                _selectedAvailableStock = value;
                OnPropertyChanged();
                ((DelegateCommand)AddStockCommand).RaiseCanExecuteChanged();
            }
        }

        public Stock SelectedAddedStock
        {
            get { return _selectedAddedStock; }
            set
            {
                _selectedAddedStock = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveStockCommand).RaiseCanExecuteChanged();
            }
        }

        public async override Task LoadAsync(int? pageId)
        {
            var page = pageId.HasValue
                ? await _pageRepository.GetByIdAsync(pageId.Value)
                : CreateNewPage();

            Id = page.Id;

            InitialisePage(page);

            _allStocks = await _pageRepository.GetAllStocksAsync();

            SetupPicklist();
        }

        private void SetupPicklist()
        {
            var pageStockIds = Page.Model.Stocks.Select(s => s.Id).ToList();
            var addedStocks = _allStocks.Where(s => pageStockIds.Contains(s.Id)).OrderBy(s => s.Symbol);
            var availableStocks = _allStocks.Except(addedStocks).OrderBy(s => s.Symbol);

            AddedStocks.Clear();
            AvailableStocks.Clear();
            foreach (var addedStock in addedStocks)
            {
                AddedStocks.Add(addedStock);
            }
            foreach (var availableStock in availableStocks)
            {
                AvailableStocks.Add(availableStock);
            }
        }

        protected override void OnDeleteExecute()
        {
            var result = MessageDialogService.ShowOkCancelDialog($"Do you really want to delete the page {Page.Title}?", "Question");
            if (result == MessageDialogResult.OK)
            {
                _pageRepository.Remove(Page.Model);
                _pageRepository.SaveAsync();
                RaiseDetailDeletedEvent(Page.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Page != null && !Page.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await _pageRepository.SaveAsync();
            HasChanges = _pageRepository.HasChanges();
            Id = Page.Id;
            RaiseDetailSavedEvent(Page.Id, Page.Title);
        }

        private bool OnRemoveStockCanExecute()
        {
            return SelectedAddedStock != null;
        }

        private void OnRemoveStockExecute()
        {
            var stockToRemove = SelectedAddedStock;

            Page.Model.Stocks.Remove(stockToRemove);
            AddedStocks.Remove(stockToRemove);
            AvailableStocks.Add(stockToRemove);
            HasChanges = _pageRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddStockCanExecute()
        {
            return SelectedAvailableStock != null;
        }

        private void OnAddStockExecute()
        {
            var stockToAdd = SelectedAvailableStock;

            Page.Model.Stocks.Add(stockToAdd);
            AddedStocks.Add(stockToAdd);
            AvailableStocks.Remove(stockToAdd);
            HasChanges = _pageRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private Page CreateNewPage()
        {
            var page = new Page();
            _pageRepository.Add(page);
            return page;
        }

        private void InitialisePage(Page page)
        {
            Page = new PageWrapper(page);
            Page.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _pageRepository.HasChanges();
                }

                if (e.PropertyName == nameof(Page.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(Page.Title))
                {
                    SetTitle();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (Page.Id == 0)
            {
                // Trigger validation
                Page.Title = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title = Page.Title;
        }
    }
}
