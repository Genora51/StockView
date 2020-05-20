﻿using Prism.Commands;
using Prism.Events;
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
        private StockSnapshotWrapper _selectedSnapshot;
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
            AddSnapshotCommand = new DelegateCommand(OnAddSnapshotExecute);
            RemoveSnapshotCommand = new DelegateCommand(OnRemoveSnapshotExecute, OnRemoveSnapshotCanExecute);

            Industries = new ObservableCollection<LookupItem>();
            Snapshots = new ObservableCollection<StockSnapshotWrapper>();
        }

        public async Task LoadAsync(int? stockId)
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


        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddSnapshotCommand { get; }
        public ICommand RemoveSnapshotCommand { get; }

        public ObservableCollection<LookupItem> Industries { get; }

        public ObservableCollection<StockSnapshotWrapper> Snapshots { get; }

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

        private void OnAddSnapshotExecute()
        {
            var newSnapshot = new StockSnapshotWrapper(new StockSnapshot {
                Date = DateTime.Now
            });
            newSnapshot.PropertyChanged += StockSnapshotWrapper_PropertyChanged;
            Snapshots.Add(newSnapshot);
            Stock.Model.Snapshots.Add(newSnapshot.Model);
        }

        private void OnRemoveSnapshotExecute()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        private bool OnRemoveSnapshotCanExecute()
        {
            return SelectedSnapshot != null;
        }
    }
}
